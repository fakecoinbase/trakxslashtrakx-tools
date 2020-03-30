using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs
{
    public class WebSocketInboundMessage
    {
        [JsonPropertyName("TYPE")] public string Type { get; set; }
    }
}