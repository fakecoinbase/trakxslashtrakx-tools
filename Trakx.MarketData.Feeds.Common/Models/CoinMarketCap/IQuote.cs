using System;

namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface IQuote
    {
        decimal Price { get; }
        double Volume24H { get; }
        double PercentChange1H { get; }
        double PercentChange24H { get; }
        double PercentChange7D { get; }
        decimal MarketCap { get; }
        DateTimeOffset LastUpdated { get; }
    }
}