using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;

namespace Trakx.MarketData.Feeds.Models.CoinMarketCap
{
    public partial class CoinsAndMarketCapListing : ICoinsAndMarketCapListing
    {
        [JsonProperty("status")]
        public IStatus Status { get; set; }

        [JsonProperty("data")]
        public ICoinAndMarketCap[] CoinsAndMarketCaps { get; set; }

        public static ICoinsAndMarketCapListing FromJson(string json) 
            => JsonConvert.DeserializeObject<CoinsAndMarketCapListing>(json, Converter.Settings);
    }
}