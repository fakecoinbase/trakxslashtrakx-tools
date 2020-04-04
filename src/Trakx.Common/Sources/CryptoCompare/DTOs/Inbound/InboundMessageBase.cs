using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.CryptoCompare.DTOs.Inbound
{
    public class InboundMessageBase
    {
        [JsonPropertyName("TYPE")] public string Type { get; set; }
    }
}