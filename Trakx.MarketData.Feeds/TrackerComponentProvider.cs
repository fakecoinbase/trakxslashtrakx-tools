using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Trakx.MarketData.Feeds.Common;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;

namespace Trakx.MarketData.Feeds
{
    public class TrackerComponentProvider : ITrackerComponentProvider
    {
        private IReadOnlyDictionary<string, ICoin> _top10;

        public TrackerComponentProvider(IParser parser)
        {
            //todo: remove hardcoded list and get the data from CoinMarketCap.com
            var coinListContent = File.ReadAllText(Path.Combine("StaticData", "cryptocompare-coinlist.json"));
            _top10 = parser.ReadCoinsBySymbol(coinListContent);
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<ICoin, decimal> GetTopXMarketCapCoins(uint coinCount)
        {
            //return _top10.ToDictionary<ICoin, decimal>((p,i) => p.Value, (p, i) => i);
            return null;
        }
    }
}
