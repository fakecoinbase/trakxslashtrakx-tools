using System;

namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface IQuote
    {
        double Price { get; }
        double Volume24H { get; }
        double PercentChange1H { get; }
        double PercentChange24H { get; }
        double PercentChange7D { get; }
        double MarketCap { get; }
        DateTimeOffset LastUpdated { get; }
    }
}