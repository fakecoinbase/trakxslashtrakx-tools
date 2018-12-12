using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        static CryptoCompare()
        {
            var coinListJsonFile = Path.Combine(Environment.CurrentDirectory, TestDataProvider.Testdata, "cryptocompare-coinlist.json");
            CoinListAsString = new Lazy<string>(() => File.ReadAllText(coinListJsonFile), LazyThreadSafetyMode.PublicationOnly);
            CoinListAsStream = () => File.OpenRead(coinListJsonFile);
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
}
