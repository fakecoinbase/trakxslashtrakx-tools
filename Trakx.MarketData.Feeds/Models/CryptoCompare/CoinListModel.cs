using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CryptoCompare;

using Trakx.MarketData.Feeds.Common.Models;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;

namespace Trakx.MarketData.Feeds.Models.CryptoCompare
{
    public class CoinListModel
    {
        public IReadOnlyDictionary<string, ICoin> CoinsBySymbol;
    }
}
