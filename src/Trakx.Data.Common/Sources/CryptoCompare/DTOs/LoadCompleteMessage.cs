using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs
{
    public class LoadCompleteMessage : WebSocketInboundMessage
    {
        public const string TypeValue = "3";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("SUB")] public string Subscription { get; set; }
    }
}