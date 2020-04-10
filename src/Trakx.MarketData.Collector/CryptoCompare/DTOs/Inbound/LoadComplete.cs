using System.Text.Json.Serialization;

namespace Trakx.MarketData.Collector.CryptoCompare.DTOs.Inbound
{
    public class LoadComplete : InboundMessageBase
    {
        internal const string TypeValue = "3";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("SUB")] public string Subscription { get; set; }
    }
}