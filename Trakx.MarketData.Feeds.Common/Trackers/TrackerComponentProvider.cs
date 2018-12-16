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
        private readonly ITrackerFactory _trackerFactory;

        public TrackerComponentProvider(ITrakxMemoryCache memoryCache, ITrackerFactory trackerFactory)
        {
            _memoryCache = memoryCache;
            _trackerFactory = trackerFactory;
        }

        /// <inheritdoc />
        public async Task<IList<string>> GetComponentTickers(string trackerTicker)
        {
            var tracker = _trackerFactory.FromTicker(trackerTicker);
            var components = await _memoryCache.GetComponentsForTracker(tracker.Ticker, tracker.ComponentExtractor);
            return components;
        }
    }
}
