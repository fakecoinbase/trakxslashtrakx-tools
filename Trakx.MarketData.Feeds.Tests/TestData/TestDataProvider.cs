using System;
using System.IO;
using System.Threading;

using CryptoCompare;

using Newtonsoft.Json;

namespace Trakx.MarketData.Feeds.Tests.TestData
{
    internal class TestDataProvider
    {
        public const string Testdata = "TestData";
    }

    internal class CryptoCompare
    {
        public static Lazy<string> CoinListAsString;
        public static Func<Stream> CoinListAsStream;

        public static Lazy<PriceMultiFullResponse> PriceMultiFullResponse;

        static CryptoCompare()
        {
            var coinListJsonFile = Path.Combine(Environment.CurrentDirectory, TestDataProvider.Testdata, "cryptocompare-coinlist.json");
            
            CoinListAsString = new Lazy<string>(() => File.ReadAllText(coinListJsonFile), LazyThreadSafetyMode.PublicationOnly);
            CoinListAsStream = () => File.OpenRead(coinListJsonFile);

            var priceMultiFullJsonFile = Path.Combine(Environment.CurrentDirectory, TestDataProvider.Testdata, "cryptocompare-pricemultifull.json");
            PriceMultiFullResponse = new Lazy<PriceMultiFullResponse>(() => JsonConvert.DeserializeObject<PriceMultiFullResponse>(File.ReadAllText(priceMultiFullJsonFile)));
        }
    }

    internal class CoinMarketCap
    {
        public static Lazy<string> CoinListAsString;
        public static Func<Stream> CoinListAsStream;

        public static Func<Stream> HistoricalMarketCap;

        static CoinMarketCap()
        {
            var coinListJsonFile = Path.Combine(Environment.CurrentDirectory, TestDataProvider.Testdata, "coinmarketcap-listing-latest.json");
            CoinListAsString = new Lazy<string>(() => File.ReadAllText(coinListJsonFile), LazyThreadSafetyMode.PublicationOnly);
            CoinListAsStream = () => File.OpenRead(coinListJsonFile);

            var historicalMarketCapFile = Path.Combine(Environment.CurrentDirectory, TestDataProvider.Testdata, "coinmarketcap-historicalrecord.html");
            HistoricalMarketCap = () => File.OpenRead(historicalMarketCapFile);
        }
    }

    internal class Messari
    {
        public static Lazy<string> CoinListAsString;
        public static Func<Stream> CoinListAsStream;
        
        static Messari()
        {
            var coinListJsonFile = Path.Combine(Environment.CurrentDirectory, TestDataProvider.Testdata, "messari-all-assets-with-profile-with-metrics.json");
            CoinListAsString = new Lazy<string>(() => File.ReadAllText(coinListJsonFile), LazyThreadSafetyMode.PublicationOnly);
            CoinListAsStream = () => File.OpenRead(coinListJsonFile);
        }
    }
}
