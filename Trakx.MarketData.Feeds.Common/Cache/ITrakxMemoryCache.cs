using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CryptoCompare;

using Microsoft.Extensions.Caching.Memory;

namespace Trakx.MarketData.Feeds.Common.Cache
{
    public interface ITrakxMemoryCache : IMemoryCache
    {
        Task<TopMarketCapResponse> Top20UsdMarketCap { get; }

        IList<string> GetComponentsForTracker(string trackerTicker, Func<ICryptoCompareClient, IList<string>> trackerComponentExtractor);
    }
}