using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.CryptoCompare.DTOs.Inbound
{
    public class HeartBeat : InboundMessageBase
    {
        internal const string TypeValue = "999";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("TIMEMS")] public ulong TimeMs { get; set; }
    }
}