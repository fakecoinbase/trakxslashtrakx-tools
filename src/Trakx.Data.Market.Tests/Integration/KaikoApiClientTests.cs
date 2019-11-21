using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Market.Common.Sources.BitGo;
using Trakx.Data.Market.Common.Sources.CryptoCompare;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;
using Trakx.Data.Market.Common.Sources.Messari.Client;
using Xunit;
using Xunit.Abstractions;
using RequestHelper = Trakx.Data.Market.Common.Sources.Kaiko.Client.RequestHelper;

namespace Trakx.Data.Market.Tests.Integration
{
    public class KaikoApiClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        //private readonly ICacheLogger<KaikoApiClient> _logger;
        private readonly RequestHelper _requestHelper;

        public KaikoApiClientTests(ITestOutputHelper output)
        {
            _output = output;
            //_logger = output.BuildLoggerFor<KaikoApiClient>();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddKaikoClient();
            serviceCollection.AddMessariClient();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _requestHelper = _serviceProvider.GetRequiredService<RequestHelper>();
            _messariClient = _serviceProvider.GetRequiredService<Common.Sources.Messari.Client.RequestHelper>();
        }


        [Fact]
        public async Task GetExchanges_should_return_exchanges()
        {
            var exchanges = await _requestHelper.GetExchanges();

            _output.WriteLine(exchanges.ToString());

            exchanges.Exchanges.ForEach(e => _output.WriteLine($"{e.Code}, {e.Name}"));
        }

        [Fact]
        public async Task GetAssets_should_return_assets()
        {
            var assets = await _requestHelper.GetAssets();

            _output.WriteLine(assets.ToString());

            assets.Assets.ForEach(a => _output.WriteLine($"{a.Code}, {a.Name}, {a.AssetClass}"));
        }

        [Fact]
        public async Task GetInstruments_should_return_instruments()
        {
            var instruments = await _requestHelper.GetInstruments();

            _output.WriteLine(instruments.ToString());

            instruments.Instruments.ForEach(i => _output.WriteLine($"{i.Code}, {i.ExchangeCode}, {i.ExchangePairCode}, {i.Class}, {i.BaseAsset}, {i.QuoteAsset}"));
        }

        [Fact]
        public async Task GetAggregatedPrice_for_one_token_should_return_aggregated_prices()
        {
            var coinSymbol = "eth";
            var query = CreateCoinQuery(coinSymbol, coinSymbol);

            var price = await _requestHelper.GetSpotExchangeRate(query, false).ConfigureAwait(false);
            var profile = (await _messariClient.GetProfileForSymbol(coinSymbol).ConfigureAwait(false)).Data;

            var results = price.Data;

            results.ForEach(r => _output.WriteLine($"{r.Price}, {profile.Sector}, {r.Volume}"));
        }


        [Fact]
        public async Task GetAggregatedPrice_should_return_aggregated_prices()
        {
            var bitGoTokens = new SupportedTokenProvider().SupportedTokensBySymbol;
            var kaikoInstruments = await _requestHelper.GetInstruments().ConfigureAwait(false);
            var kaikoErc20Symbols = kaikoInstruments.Instruments.Select(i => i.Code.Split("-")[0].ToLower())
                .Distinct()
                .OrderBy(s => s)
                .Intersect(bitGoTokens.Keys)
                .Take(20)
                .ToList();

            var assetNameByCode = (await _requestHelper.GetAssets().ConfigureAwait(false))
                .Assets.ToDictionary(a => a.Code, a => a.Name);

            var quoteAsset = "usd";
            
            var queries = kaikoErc20Symbols.Select(e => CreateCoinQuery(e, quoteAsset)).ToList();

            var tempPath = "kaikoData." + DateTime.Now.ToString("yyyyMMdd.hhmmss");
            Directory.CreateDirectory(tempPath);
            var priceTasks = queries.AsParallel().Select(async q =>
                {
                    var aggregatedPrice = await _requestHelper.GetSpotExchangeRate(q).ConfigureAwait(false);
                    var profile = await _messariClient.GetProfileForSymbol(q.BaseAsset).ConfigureAwait(false);
                    if (aggregatedPrice?.Result == "success" && aggregatedPrice.Data.Any())
                    {
                        File.WriteAllText(Path.Combine(tempPath, q.BaseAsset + ".json"), 
                            JsonSerializer.Serialize(aggregatedPrice));
                        return new { 
                            Query = q, 
                            Prices = aggregatedPrice, 
                            Sector = profile?.Data?.Sector ?? "", 
                            BitGoCustody = bitGoTokens.ContainsKey(q.BaseAsset.ToLower())
                        };
                    }
                    return null;
                })
                .Where(r => r?.Result != null)
                .ToArray();

            var results = priceTasks.Select(p => p.Result).Where(r => r != null).OrderBy(r => r.Query.BaseAsset).ToList();
            _output.WriteLine($"symbol, name, sector, volume ({quoteAsset}), price ({quoteAsset}), bitgoCustody");
            results.ForEach(r =>
               {
                   var summedVolume = r.Prices.Data.Sum(a => decimal.Parse(a.Volume));
                   var averagePrice = r.Prices.Data.Average(a => decimal.Parse(a.Price) * decimal.Parse(a.Volume)) / summedVolume;
                   var symbol = r.Query.BaseAsset;
                   var assetName = assetNameByCode[symbol];
                   _output.WriteLine($"{symbol}, {assetName}, {r.Sector}, {summedVolume}, {averagePrice}, {r.BitGoCustody}");
               });
        }

        private AggregatedPriceRequest CreateCoinQuery(string coinSymbol, string quoteSymbol)
        {
            var query = new AggregatedPriceRequest
            {
                DataVersion = "latest",
                BaseAsset = coinSymbol.ToLower(),
                Commodity = "trades",
                Exchanges = new List<string>(),
                Interval = "1d",
                PageSize = 1000,
                QuoteAsset = quoteSymbol,
                StartTime = new DateTimeOffset(2019, 10, 01, 00, 00, 00, TimeSpan.Zero),
                Sources = true
            };
            return query;
        }

        private readonly ServiceProvider _serviceProvider;
        private readonly Common.Sources.Messari.Client.RequestHelper _messariClient;

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}
