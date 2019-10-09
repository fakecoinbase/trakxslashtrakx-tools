using System.Collections.Generic;

namespace Trakx.MarketApi.DataSources.Kaiko
{
    public static class TrustedExchanges
    {
        /*
        System.Collections.Generic.List`1[Trakx.MarketApi.DataSources.CoinGecko.ExchangeDetails]
        0, 0, Binance, binance
        0, 0, Kraken, kraken
        0, 0, Bitfinex, bitfinex
        0, 0, KuCoin, kucoin
        0, 0, Poloniex, poloniex
        0, 0, Bittrex, bittrex
        0, 0, Upbit, upbit
        0, 0, Coinbase Pro, gdax
        0, 0, Huobi Global, huobi
        0, 0, Bitstamp, bitstamp
        */

        public static List<string> Symbols = new List<string>
        {
            "bnce",
            "krkn",
            "bfnx",
            "kcon",
            "polo",
            "btrx",
            "upbt",
            "cbse",
            "huob",
            "stmp"
        };

        public static string SymbolsAsCsv => string.Join(",", Symbols);
    }
}
