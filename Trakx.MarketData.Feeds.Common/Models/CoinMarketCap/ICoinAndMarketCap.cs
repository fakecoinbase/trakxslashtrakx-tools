using System;
using System.Collections.Generic;

namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface ICoinAndMarketCap
    {
        long Id { get; }
        string Name { get; }
        string Symbol { get; }
        string Slug { get; } 
        double CirculatingSupply { get; }
        double TotalSupply { get; }
        long? MaxSupply { get; }
        DateTimeOffset DateAdded { get; }
        long NumMarketPairs { get; }
        long CmcRank { get; }
        DateTimeOffset LastUpdated { get; }
        IDictionary<string, IQuote> Quote { get; }
    }
}