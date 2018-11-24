using System;

using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;

namespace Trakx.MarketData.Feeds.Models.CoinMarketCap
{
    public partial class Quote : IQuote
    {
        public Quote(
            double price,
            double volume24H,
            double percentChange1H,
            double percentChange24H,
            double percentChange7D,
            double marketCap,
            DateTimeOffset lastUpdated)
        {
            Price = price;
            Volume24H = volume24H;
            PercentChange1H = percentChange1H;
            PercentChange24H = percentChange24H;
            PercentChange7D = percentChange7D;
            MarketCap = marketCap;
            LastUpdated = lastUpdated;
        }

        [JsonProperty("price")]
        public double Price { get; }

        [JsonProperty("volume_24h")]
        public double Volume24H { get; }

        [JsonProperty("percent_change_1h")]
        public double PercentChange1H { get; }

        [JsonProperty("percent_change_24h")]
        public double PercentChange24H { get; }

        [JsonProperty("percent_change_7d")]
        public double PercentChange7D { get; }

        [JsonProperty("market_cap")]
        public double MarketCap { get; }

        [JsonProperty("last_updated")]
        public DateTimeOffset LastUpdated { get; }
    }
}