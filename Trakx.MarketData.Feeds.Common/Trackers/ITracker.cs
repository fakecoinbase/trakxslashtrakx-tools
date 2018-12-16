using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Trackers
{
    public interface ITracker
    {
        string Ticker { get; }
        int Leverage { get; }
        string Symbol { get; }
        int BasketSize { get; }
        Func<ICryptoCompareClient, Task<IList<string>>> ComponentExtractor { get; }
    }
}