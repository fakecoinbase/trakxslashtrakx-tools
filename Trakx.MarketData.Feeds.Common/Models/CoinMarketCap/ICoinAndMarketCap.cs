using System;
using System.Collections.Generic;

namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface ICoinAndMarketCap
    {
        long Id { get; set; }
        string Name { get; set; }
        string Symbol { get; set; }
        string Slug { get; set; }
        double CirculatingSupply { get; set; }
        double TotalSupply { get; set; }
        long? MaxSupply { get; set; }
        DateTimeOffset DateAdded { get; set; }
        long NumMarketPairs { get; set; }
        long CmcRank { get; set; }
        DateTimeOffset LastUpdated { get; set; }
        Dictionary<string, IQuote> Quote { get; set; }
    }
}