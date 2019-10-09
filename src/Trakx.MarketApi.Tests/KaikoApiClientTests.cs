using System;
using System.Linq;
using System.Threading.Tasks;
using Trakx.MarketApi.DataSources.CryptoCompare;
using Trakx.MarketApi.DataSources.Kaiko;
using Trakx.MarketApi.DataSources.Kaiko.AggregatedPrice;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketApi.Tests
{
    public class KaikoApiClientTests
    {
        private readonly ITestOutputHelper _output;

        public KaikoApiClientTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetExchanges_should_return_exchanges()
        {
            var kaikoApi = new KaikoApiClient();
            var exchanges = await kaikoApi.GetExchanges();

            _output.WriteLine(exchanges.ToString());

            exchanges.Data.ForEach(e => _output.WriteLine($"{e.Code}, {e.Name}"));
        }

        [Fact]
        public async Task GetAssets_should_return_assets()
        {
            var kaikoApi = new KaikoApiClient();
            var assets = await kaikoApi.GetAssets();

            _output.WriteLine(assets.ToString());

            assets.Data.ForEach(a => _output.WriteLine($"{a.Code}, {a.Name}, {a.AssetClass}"));
        }


        [Fact]
        public async Task GetInstruments_should_return_instruments()
        {
            var kaikoApi = new KaikoApiClient();
            var instruments = await kaikoApi.GetInstruments();

            _output.WriteLine(instruments.ToString());

            instruments.Data.ForEach(i => _output.WriteLine($"{i.Code}, {i.ExchangeCode}, {i.ExchangePairCode}, {i.Class}, {i.BaseAsset}, {i.QuoteAsset}"));
        }

        [Fact]
        public async Task GetAggregatedPrice_for_one_token_should_return_aggregated_prices()
        {
            var query = CreateCoinQuery("OMG");

            var kaikoApi = new KaikoApiClient();
            var price = await kaikoApi.GetAggregatedPrice(query).ConfigureAwait(false);

            var results = price.Data;

            results.ForEach(r => _output.WriteLine($"{r.Price}, {r.Volume}"));
        }


        [Fact]
        public async Task GetAggregatedPrice_should_return_aggregated_prices()
        {
            var cryptoCompareCoins = new CryptoCompareApiClient();
            var erc20Symbols = cryptoCompareCoins.GetAllErc20Symbols();

            var queries = erc20Symbols.Select(CreateCoinQuery).ToList();

            var kaikoApi = new KaikoApiClient();
            var priceTasks = queries.Select(async q =>
                {
                    var aggregatedPrice = await kaikoApi.GetAggregatedPrice(q).ConfigureAwait(false);
                    if(aggregatedPrice.Result == "success") return aggregatedPrice;
                    return null;
                })
                .Where(r => r != null)
                .ToArray();

            var results = priceTasks.Select(p => p.Result.Data).ToList();

           results.ForEach(r => _output.WriteLine(string.Join(",", r.Select(a => a.Volume))));
        }

        private Query CreateCoinQuery(string coinSymbol)
        {
            var query = new Query
            {
                DataVersion = "latest",
                BaseAsset = coinSymbol.ToLower(),
                Commodity = "trades",
                Exchanges = TrustedExchanges.Symbols,
                Interval = "1d",
                PageSize = 1000,
                QuoteAsset = "usd",
                StartTime = new DateTimeOffset(2019, 10, 01, 00, 00, 00, TimeSpan.Zero),
                Sources = true
            };
            return query;
        }
    }
}
