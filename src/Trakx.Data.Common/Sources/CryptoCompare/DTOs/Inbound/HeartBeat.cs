using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs.Inbound
{
    public class HeartBeat : InboundMessageBase
    {
        public const string TypeValue = "999";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("TIMEMS")] public ulong TimeMs { get; set; }
    }
}