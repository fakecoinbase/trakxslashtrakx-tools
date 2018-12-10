using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CryptoCompare;

using Microsoft.Extensions.Caching.Memory;

using Trakx.MarketData.Feeds.Common.Cache;
using Trakx.MarketData.Feeds.Common.Trackers;

namespace Trakx.MarketData.Feeds.Common.Models.Trakx
{
    public class TrackerComponentProvider : ITrackerComponentProvider
    {
        private readonly ITrakxMemoryCache _memoryCache;

        public TrackerComponentProvider(ITrakxMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <inheritdoc />
        public async Task<IList<string>> GetComponentTickers(string trackerTicker)
        {
            var tracker = new Tracker(trackerTicker);
            var components = _memoryCache.GetComponentsForTracker(tracker.Ticker, tracker.ComponentExtractor);
            return null;
        }
    }
}
