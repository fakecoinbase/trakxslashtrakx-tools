using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Trakx.MarketData.Feeds.Common.ApiClients;

namespace Trakx.MarketData.Feeds.ApiClients
{
    public class CoinSymbolMapper : ICoinSymbolMapper
    {
        private static ReadOnlyDictionary<string, string> _cryptoCompareToCoinMarketCap;
        private static ReadOnlyDictionary<string, string> _coinMarketCapToCryptoCompare;
        
        static CoinSymbolMapper()
        {
            _cryptoCompareToCoinMarketCap =
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() { { "IOT", "MIOTA" } });
            _coinMarketCapToCryptoCompare =
                new ReadOnlyDictionary<string, string>(_cryptoCompareToCoinMarketCap.ToDictionary(p => p.Value, p => p.Key));

        }

        public string CryptoCompareToCoinMarketCap(string cryptoCompareSymbol)
        {
            return _cryptoCompareToCoinMarketCap
                       .TryGetValue(cryptoCompareSymbol, out string coinMarketCapSymbol) 
                       ? coinMarketCapSymbol : cryptoCompareSymbol;
        }

        public string CoinMarketCapToCryptoCompare(string coinMarketCapSymbol)
        {
            return _coinMarketCapToCryptoCompare
                       .TryGetValue(coinMarketCapSymbol, out string cryptoCompareSymbol)
                       ? cryptoCompareSymbol : coinMarketCapSymbol;
        }
    }
}
