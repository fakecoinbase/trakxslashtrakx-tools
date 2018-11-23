using System;

namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface IQuote
    {
        double Price { get; set; }
        double Volume24H { get; set; }
        double PercentChange1H { get; set; }
        double PercentChange24H { get; set; }
        double PercentChange7D { get; set; }
        double MarketCap { get; set; }
        DateTimeOffset LastUpdated { get; set; }
    }
}