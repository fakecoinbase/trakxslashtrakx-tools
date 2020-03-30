using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs
{
    public class ErrorMessage : WebSocketInboundMessage
    {
        public const string TypeValue = "500";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("PARAMETER")] public string Parameter { get; set; }
        [JsonPropertyName("INFO")] public string Info { get; set; }
    }
}