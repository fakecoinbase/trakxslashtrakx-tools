using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Market.Common.Indexes;
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
        private readonly IKaikoClient _kaikoClient;

        public KaikoApiClientTests(ITestOutputHelper output)
        {
            _output = output;
            //_logger = output.BuildLoggerFor<KaikoApiClient>();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddKaikoClient();
            serviceCollection.AddMessariClient();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _kaikoClient = _serviceProvider.GetRequiredService<IKaikoClient>();
            _messariClient = _serviceProvider.GetRequiredService<Common.Sources.Messari.Client.RequestHelper>();
        }


        [Fact]
        public async Task GetExchanges_should_return_exchanges()
        {
            var exchanges = await _kaikoClient.GetExchanges();

            _output.WriteLine(exchanges.ToString());

            exchanges.Exchanges.ForEach(e => _output.WriteLine($"{e.Code}, {e.Name}"));
        }

        [Fact]
        public async Task GetInstruments_should_return_instruments()
        {
            var instruments = await _kaikoClient.GetInstruments();

            _output.WriteLine(instruments.ToString());

            instruments.Instruments.ForEach(i => _output.WriteLine($"{i.Code}, {i.ExchangeCode}, {i.ExchangePairCode}, {i.Class}, {i.BaseAsset}, {i.QuoteAsset}"));
        }

        [Fact]
        public async Task GetAggregatedPrice_for_one_token_should_return_aggregated_prices()
        {
            var coinSymbol = "btc";
            var quoteSymbol = "usd";
            var query = CreateCoinQuery(coinSymbol, quoteSymbol, false);

            var price = await _kaikoClient.GetSpotExchangeRate(query).ConfigureAwait(false);
            var profile = (await _messariClient.GetProfileForSymbol(coinSymbol).ConfigureAwait(false)).Data;

            var results = price.Data;

            results.ForEach(r => _output.WriteLine($"{r.Price}, {profile.Sector}, {r.Volume}"));
        }


        [Fact(Skip = "This is a long running one, actually not for tests")]
        public async Task GetAggregatedPrice_should_return_aggregated_prices()
        {
            //todo: usd eur bnb btc eth 

            var bitGoTokens = new SupportedTokenProvider().SupportedTokensBySymbol;
            var indexTokens = new IndexDetailsProvider().IndexDetails
                .SelectMany(i => i.Value.Components.Select(c => c.Symbol.ToLower())).Distinct().ToList();

            var kaikoInstruments = await _kaikoClient.GetInstruments().ConfigureAwait(false);
            var symbols = kaikoInstruments.Instruments.Select(i => i.Code.Split("-")[0].ToLower())
                .Distinct()
                .OrderBy(s => s);

            var quoteAsset = "usd";
            
            var queries = symbols.Select(e => CreateCoinQuery(e, quoteAsset, false)).ToList();

            var tempPath = "kaikoData." + DateTime.Now.ToString("yyyyMMdd.hhmmss");
            Directory.CreateDirectory(tempPath);
            var priceTasks = queries.AsParallel().Select(async q =>
                {
                    var aggregatedPrice = await _kaikoClient.GetSpotExchangeRate(q).ConfigureAwait(false);
                    var profile = await _messariClient.GetProfileForSymbol(q.BaseAsset).ConfigureAwait(false);
                    if (aggregatedPrice?.Result == "success" && aggregatedPrice.Data.Any())
                    {
                        File.WriteAllText(Path.Combine(tempPath, q.BaseAsset + ".json"), 
                            JsonSerializer.Serialize(aggregatedPrice));
                        return new { 
                            Query = q, 
                            Prices = aggregatedPrice, 
                            Sector = profile?.Data?.Sector ?? "", 
                            BitGoCustody = bitGoTokens.ContainsKey(q.BaseAsset.ToLower()),
                            UsedByTrakx = indexTokens.Contains(q.BaseAsset.ToLower()),
                            AssetName = profile?.Data?.Name ?? "",
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
                   var averagePrice = r.Prices.Data.Average(a => decimal.Parse(a.Price));
                   var symbol = r.Query.BaseAsset;
                   _output.WriteLine($"{symbol}, {r.AssetName}, {r.Sector}, {summedVolume}, {averagePrice}, {r.BitGoCustody}");
               });
        }

        private AggregatedPriceRequest CreateCoinQuery(string coinSymbol, string quoteSymbol, bool direct)
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
                Sources = false,
                DirectExchangeRate = direct
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
