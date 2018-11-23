using System;

namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface IStatus
    {
        DateTimeOffset Timestamp { get; set; }
        long ErrorCode { get; set; }
        object ErrorMessage { get; set; }
        long Elapsed { get; set; }
        long CreditCount { get; set; }
    }
}