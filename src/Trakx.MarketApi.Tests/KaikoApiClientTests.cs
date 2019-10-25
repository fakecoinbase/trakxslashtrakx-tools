using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Trakx.MarketApi.DataSources.CryptoCompare;
using Trakx.MarketApi.DataSources.Kaiko;
using Trakx.MarketApi.DataSources.Kaiko.Client;
using Trakx.MarketApi.DataSources.Kaiko.DTOs;
using Trakx.MarketApi.DataSources.Messari.Client;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketApi.Tests
{
    public class KaikoApiClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly ICacheLogger<KaikoApiClient> _logger;
        private readonly KaikoApiClient _kaikoApiClient;

        public KaikoApiClientTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = output.BuildLoggerFor<KaikoApiClient>();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddKaikoClient();
            serviceCollection.AddMessariClient();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _kaikoApiClient = _serviceProvider.GetRequiredService<KaikoApiClient>();
            _messariClient = _serviceProvider.GetRequiredService<MessariApiClient>();
        }


        [Fact]
        public async Task GetExchanges_should_return_exchanges()
        {
            var exchanges = await _kaikoApiClient.GetExchanges();

            _output.WriteLine(exchanges.ToString());

            exchanges.Data.ForEach(e => _output.WriteLine($"{e.Code}, {e.Name}"));
        }

        [Fact]
        public async Task GetAssets_should_return_assets()
        {
            var assets = await _kaikoApiClient.GetAssets();

            _output.WriteLine(assets.ToString());

            assets.Data.ForEach(a => _output.WriteLine($"{a.Code}, {a.Name}, {a.AssetClass}"));
        }


        [Fact]
        public async Task GetInstruments_should_return_instruments()
        {
            var instruments = await _kaikoApiClient.GetInstruments();

            _output.WriteLine(instruments.ToString());

            instruments.Data.ForEach(i => _output.WriteLine($"{i.Code}, {i.ExchangeCode}, {i.ExchangePairCode}, {i.Class}, {i.BaseAsset}, {i.QuoteAsset}"));
        }

        [Fact]
        public async Task GetAggregatedPrice_for_one_token_should_return_aggregated_prices()
        {
            var query = CreateCoinQuery("celr", "btc");

            var price = await _kaikoApiClient.GetAggregatedPrice(query).ConfigureAwait(false);

            var results = price.Data;

            results.ForEach(r => _output.WriteLine($"{r.Price}, {r.Volume}"));
        }


        [Fact]
        public async Task GetAggregatedPrice_should_return_aggregated_prices()
        {
            var cryptoCompareCoins = new CryptoCompareApiClient();
            var ccErc20Symbols = cryptoCompareCoins.GetAllErc20Symbols()
                .Select(c => c.ToLower()).OrderBy(s => s).ToList();

            ccErc20Symbols.Should().Contain("celr");

            var kaikoInstruments = await _kaikoApiClient.GetInstruments().ConfigureAwait(false);
            var kaikoErc20Symbols = kaikoInstruments.Data.Select(i => i.Code.Split("-")[0].ToLower())
                .Distinct()
                .Intersect(ccErc20Symbols)
                .OrderBy(s => s)
                .ToList();

            var assetNameByCode = (await _kaikoApiClient.GetAssets().ConfigureAwait(false))
                .Data.ToDictionary(a => a.Code, a => a.Name);

            var messariAssetDetails = (await _messariClient.GetAllAssets()).Data.ToDictionary(a => a.Symbol.ToLower(), a => a.Profile);

            var quoteAsset = "btc";
            
            var queries = kaikoErc20Symbols.Select(e => CreateCoinQuery(e, quoteAsset)).ToList();

            var tempPath = "kaikoData." + DateTime.Now.ToString("yyyyMMdd.hhmmss");
            Directory.CreateDirectory(tempPath);
            var priceTasks = queries.Take(20).AsParallel().Select(async q =>
                {
                    var aggregatedPrice = await _kaikoApiClient.GetAggregatedPrice(q).ConfigureAwait(false);
                    if (aggregatedPrice?.Result == "success" && aggregatedPrice.Data.Any())
                    {
                        File.WriteAllText(Path.Combine(tempPath, q.BaseAsset + ".json"), JsonConvert.SerializeObject(aggregatedPrice.Data));
                        return new { Query = q, Prices = aggregatedPrice};
                    }
                    return null;
                })
                .Where(r => r?.Result != null)
                .ToArray();

            var results = priceTasks.Select(p => p.Result).Where(r => r != null).OrderBy(r => r.Query.BaseAsset).ToList();
            _output.WriteLine($"symbol, name, sector, volume ({quoteAsset}), price ({quoteAsset})");
            results.ForEach(r =>
               {
                   var summedVolume = r.Prices.Data.Sum(a => decimal.Parse(a.Volume));
                   var averagePrice = r.Prices.Data.Average(a => decimal.Parse(a.Price) * decimal.Parse(a.Volume)) / summedVolume;
                   var symbol = r.Query.BaseAsset;
                   var assetName = assetNameByCode[symbol];
                   var sector = messariAssetDetails.TryGetValue(symbol, out var profile) ? profile.Sector : "";
                   _output.WriteLine($"{symbol}, {assetName}, {sector}, {summedVolume}, {averagePrice}");
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
                Sources = false
            };
            return query;
        }

        private readonly ServiceProvider _serviceProvider;
        private readonly MessariApiClient _messariClient;

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}
