using System;

namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface IStatus
    {
        DateTimeOffset Timestamp { get; }
        long ErrorCode { get; }
        object ErrorMessage { get; }
        long Elapsed { get; }
        long CreditCount { get; }
    }
}