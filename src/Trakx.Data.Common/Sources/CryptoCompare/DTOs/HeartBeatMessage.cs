using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs
{
    public class HeartBeatMessage : WebSocketInboundMessage
    {
        public const string TypeValue = "999";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("TIMEMS")] public ulong TimeMs { get; set; }
    }
}