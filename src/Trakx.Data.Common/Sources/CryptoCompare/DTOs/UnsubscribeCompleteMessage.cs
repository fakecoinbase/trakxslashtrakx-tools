using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs
{
    public class UnsubscribeCompleteMessage : WebSocketInboundMessage
    {
        public const string TypeValue = "17";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("PARAMETER")] public string Parameter { get; set; }
        [JsonPropertyName("INFO")] public string Info { get; set; }
    }
}