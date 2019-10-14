using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Divergic.Logging.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Trakx.MarketApi.DataSources.CryptoCompare;
using Trakx.MarketApi.DataSources.Kaiko;
using Trakx.MarketApi.DataSources.Kaiko.AggregatedPrice;
using Trakx.MarketApi.DataSources.Kaiko.Client;
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
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _kaikoApiClient = _serviceProvider.GetRequiredService<KaikoApiClient>();
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
            var query = CreateCoinQuery("OMG");

            var price = await _kaikoApiClient.GetAggregatedPrice(query).ConfigureAwait(false);

            var results = price.Data;

            results.ForEach(r => _output.WriteLine($"{r.Price}, {r.Volume}"));
        }


        [Fact]
        public async Task GetAggregatedPrice_should_return_aggregated_prices()
        {
            var cryptoCompareCoins = new CryptoCompareApiClient();
            var erc20Symbols = cryptoCompareCoins.GetAllErc20Symbols()
                .Select(c => c.ToLower()).Intersect(WorkingTokens);

            var queries = erc20Symbols.Select(CreateCoinQuery).ToList();

            Directory.CreateDirectory("kaikodata");
            var priceTasks = queries.AsParallel().Select(async q =>
                {
                    var aggregatedPrice = await _kaikoApiClient.GetAggregatedPrice(q).ConfigureAwait(false);
                    if (aggregatedPrice?.Result == "success" && aggregatedPrice.Data.Any())
                    {
                        File.WriteAllText(Path.Combine("kaikodata", q.BaseAsset + ".json"), JsonConvert.SerializeObject(aggregatedPrice.Data));
                        return new { Query = q, Prices = aggregatedPrice};
                    }
                    return null;
                })
                .Where(r => r?.Result != null)
                .ToArray();

            var results = priceTasks.Select(p => p.Result).Where(r => r != null).ToList();

           results.ForEach(r =>
           {
               var summedVolume = r.Prices.Data.Sum(a => decimal.Parse(a.Volume));
               var averagePrice = r.Prices.Data.Average(a => decimal.Parse(a.Price) * decimal.Parse(a.Volume)) / summedVolume;
               _output.WriteLine($"{r.Query.BaseAsset}, {summedVolume}, {averagePrice}");
           });
        }

        private AggregatedPriceRequest.QueryParameters CreateCoinQuery(string coinSymbol)
        {
            var query = new AggregatedPriceRequest.QueryParameters
            {
                DataVersion = "latest",
                BaseAsset = coinSymbol.ToLower(),
                Commodity = "trades",
                Exchanges = Constants.TrustedExchanges,
                Interval = "1d",
                PageSize = 1000,
                QuoteAsset = "usd",
                StartTime = new DateTimeOffset(2019, 10, 01, 00, 00, 00, TimeSpan.Zero),
                Sources = true
            };
            return query;
        }

        private List<string> WorkingTokens = new List<string>
        {
            "rep", "bnt", "sngls", "gnt", "mkr", "mln", "rlc", "gno", "bat", "qtum", "snt", "avt", "san", "fun", "zrx",
            "storj", "omg", "mana", "utk", "ctx", "wtc", "vee", "rcn", "link", "edo", "knc", "lrc", "yoyow", "req",
            "dat", "ast", "cnd", "wax", "wpr", "drgn", "atm", "aid", "qash", "ufr", "spank", "abyss", "utnp", "tnb",
            "mgo", "elf", "onl", "int", "cbt", "agi", "bft", "dta", "iost", "zil", "poly", "swm", "hbz", "lym", "fsn",
            "dai", "ht", "nec", "dadi", "dth", "auc", "ncash", "loom", "tusd", "mith", "pas", "ctxc", "vld", "ors",
            "cnn", "ode", "hot", "ants", "dgx", "scrl", "atmi", "seer", "zb", "eurs", "box", "kan", "dgtx", "trio",
            "usdc", "pax", "foam", "pxg", "dusk", "wbtc", "usds", "mrs", "yeed", "voco", "ampl", "ftxt", "xchf", "pnk",
        };

        private readonly ServiceProvider _serviceProvider;

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }
    }
}
