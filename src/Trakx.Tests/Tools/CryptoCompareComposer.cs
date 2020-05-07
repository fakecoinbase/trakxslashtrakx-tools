using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Ethereum;
using Trakx.Common.Extensions;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Messari.Client;
using Trakx.MarketData.Collector.CryptoCompare;
using Trakx.Tests.Unit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Tools
{
    [Collection(nameof(SeededDbContextCollection))]
    public class CryptoCompareComposer
    {
        private const string QuoteSymbol = "USDC";

        private readonly ITestOutputHelper _output;
        private readonly ICryptoCompareClient _cryptoCompareClient;
        private readonly ICoinGeckoClient _coinGeckoClient;
        private readonly SeededInMemoryIndiceRepositoryContext _dbContext;


        private readonly Dictionary<string, List<string>> SymbolsByIndice =
            new Dictionary<string, List<string>>
        {
            {"l1cex", new List<string>{"FTT", "HT", "KCS", "LEO", "OKB"}},
            {"l1dex", new List<string>{"KNC", "LRC", "ZRX"}},
            {"l1mcap10erc20", new List<string>{"FTT", "HT", "MKR", "LEO", "OKB", "LINK", "OMG", "KNC", "ZRX", "KCS", "NEXO", "MATIC", "SEELE", "BAT"}},
            {"l1btceth", new List<string>{"BTC", "ETH"}},
        };

        private readonly Dictionary<string, decimal> TotalSupplyBySymbols =
            new Dictionary<string, decimal>
            {
                { "FTT", 346529399m },
                { "HT",  226294792.971921m },
                { "KCS", 81850451m },
                { "LEO", 660000000m},
                { "OKB", 60000000m },
                { "KNC", 179625326.229498m },
                { "LRC", 1027982863.75338m },
                { "ZRX", 651704448.042216m },
                { "MKR", 1005968.95438901m },
                { "LINK", 350000000m },
                { "OMG", 140245398.245133m },
                { "NEXO", 560000011m },
                { "MATIC", 2758503686m },
                { "SEELE", 699587206.289478m },
                { "BAT", 1442992563.5663m },
                { "BTC", 18298112m },
                { "ETH", 110331171.4365m },
            };

        private readonly List<string> _exchanges = new List<string>
        {
            //top 10
            "Bitstamp", "Bittrex", "Poloniex", "Kraken", "Bitfinex",
            "Coinbase", "itBit", "Gemini", "Binance","Coinbase", "bitFlyer",
            //the block 22
            "Coinfloor","Huobi US", "Coinfloor", "LMAX Digital", "Bitbank", "Bithumb Korea",
            "Coincheck", "Zaif", "OKEX", "UPbit", "Kucoin", "Liquid", "Gate.io"
        };


        public CryptoCompareComposer(SeededDbContextFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _dbContext = fixture.Context;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMessariClient();
            serviceCollection.AddCryptoCompareClient();
            serviceCollection.AddSingleton<IApiDetailsProvider>(new ApiDetailsProvider(Secrets.CryptoCompareApiKey));
            serviceCollection.AddCoinGeckoClient();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddEthereumInteraction(Secrets.InfuraApiKey);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _coinGeckoClient = serviceProvider.GetService<ICoinGeckoClient>();
            _cryptoCompareClient = serviceProvider.GetService<ICryptoCompareClient>();
        }

        //[Fact]
        [Fact(Skip = "Not a test")]
        public async Task GetCryptoCompareHistoricalMarketCaps()
        {
            var britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var asOf = TimeZoneInfo.ConvertTimeToUtc(new DateTime(2020, 04, 30, 18, 00, 00), britishZone);
            
            var symbols = SymbolsByIndice.Values.SelectMany(v => v).Distinct().ToList();

            var pricesBySymbol = await GetHistoricalPricesAsOf(asOf, symbols).ConfigureAwait(false);
            
            //var marketCapsBySymbol = await GetMarketCapsBySymbol(asOf, symbols).ConfigureAwait(false);
            
            _output.WriteLine(JsonSerializer.Serialize(symbols.Select(s => new
                {
                    Symbol = s,
                    Price = pricesBySymbol[s],
                    MarketCap = TotalSupplyBySymbols[s] * pricesBySymbol[s]
                }),
                new JsonSerializerOptions { WriteIndented = true }));

        }

        private async Task<Dictionary<string, decimal>> GetHistoricalPricesAsOf(DateTime asOf, List<string> symbols)
        {
            var priceFetchTasks = symbols.Select(tokenSymbol =>
                    _cryptoCompareClient.History.HistoricalForTimestampAsync(
                        tokenSymbol, new[] {QuoteSymbol}, asOf, _exchanges, CalculationType.MidHighLow))
                .ToList();
            
            await Task.WhenAll(priceFetchTasks);

            var result = priceFetchTasks.ToDictionary(
                t => t.Result.Keys.Single(),
                t => t.Result.Values.Single()[QuoteSymbol]);

            return result;
        }

        private async Task<Dictionary<string?, decimal?>> GetMarketCapsBySymbol(DateTime asOf, List<string> symbols)
        {
            var priceFetchTasks = symbols
                .Select(s => _dbContext.ComponentDefinitions
                    .Single(c => c.Symbol.Equals(s.ToWrappedSymbol(), StringComparison.InvariantCultureIgnoreCase)).CoinGeckoId)
                .Select(id => _coinGeckoClient.GetMarketDataAsOfFromId(id, asOf))
                .ToList();
            
            await Task.WhenAll(priceFetchTasks);

            var result = priceFetchTasks.ToDictionary(
                t => t.Result.CoinSymbol?.ToNativeSymbol().ToUpperInvariant(), 
                t => t.Result.MarketCap);
            return result;
        }
    }
}