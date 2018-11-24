using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.Converters;
using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;

namespace Trakx.MarketData.Feeds.Models.CoinMarketCap
{
    public partial class CoinsAndMarketCapListing : ICoinsAndMarketCapListing
    {
        public CoinsAndMarketCapListing(IStatus status, ICoinAndMarketCap[] coinsAndMarketCaps)
        {
            Status = status;
            CoinsAndMarketCaps = coinsAndMarketCaps;
        }

        [JsonProperty("status")]
        [JsonConverter(typeof(ConcreteConverter<Status>))]
        public IStatus Status { get; }

        [JsonProperty("data")]
        [JsonConverter(typeof(ConcreteConverter<CoinAndMarketCap[]>))]
        public ICoinAndMarketCap[] CoinsAndMarketCaps { get; }

        public static ICoinsAndMarketCapListing FromJson(string json) 
            => JsonConvert.DeserializeObject<CoinsAndMarketCapListing>(json, Converter.Settings);
    }
}