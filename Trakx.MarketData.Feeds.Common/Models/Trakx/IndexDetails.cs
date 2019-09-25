using Newtonsoft.Json;

namespace Trakx.MarketData.Feeds.Common.Models.Trakx
{
    public partial class IndexDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("components")]
        public Component[] Components { get; set; }

        [JsonProperty("componentAddresses")]
        public string[] ComponentAddresses { get; set; }

        [JsonProperty("targetUsdPrice")]
        public double TargetUsdPrice { get; set; }

        [JsonProperty("usdBidAsk")]
        public UsdBidAsk UsdBidAsk { get; set; }
    }

    public partial class Component
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("decimals")]
        public long Decimals { get; set; }

        [JsonProperty("usdBidAsk")]
        public UsdBidAsk UsdBidAsk { get; set; }

        [JsonProperty("proportion")]
        public string Proportion { get; set; }
    }

    public partial class UsdBidAsk
    {
        [JsonProperty("bid")]
        public string Bid { get; set; }

        [JsonProperty("ask")]
        public string Ask { get; set; }
    }
}
