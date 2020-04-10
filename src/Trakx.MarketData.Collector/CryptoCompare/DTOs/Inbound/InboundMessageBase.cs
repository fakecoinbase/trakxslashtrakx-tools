using System.Text.Json.Serialization;

namespace Trakx.MarketData.Collector.CryptoCompare.DTOs.Inbound
{
    public class InboundMessageBase
    {
        [JsonPropertyName("TYPE")] public string Type { get; set; }
    }
}