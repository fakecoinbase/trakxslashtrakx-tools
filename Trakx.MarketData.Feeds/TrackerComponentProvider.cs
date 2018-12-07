using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Trakx.MarketData.Feeds.Common;
using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;
using Trakx.MarketData.Feeds.Common.Models.Trakx;

namespace Trakx.MarketData.Feeds
{
    public class TrackerComponentProvider : ITrackerComponentProvider
    {
        private readonly ICryptoCompareApiClient _cryptoCompareApiClient;


        private IReadOnlyDictionary<string, ICoin> _top10;

        public TrackerComponentProvider(
            ICryptoCompareApiClient cryptoCompareApiClient)
        {
            _cryptoCompareApiClient = cryptoCompareApiClient;
        }

        /// <inheritdoc />
        public async Task<IList<ICryptoCompareCoinAndMarketCap>> GetTopXMarketCapCoins(uint coinCount)
        {
            var cryptoCompareCoins = await _cryptoCompareApiClient.GetAllSupportedCoins();


            return null;
        }
    }
}
