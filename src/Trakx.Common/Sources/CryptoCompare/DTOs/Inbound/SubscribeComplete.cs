using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.CryptoCompare.DTOs.Inbound
{
    public class SubscribeComplete : InboundMessageBase
    {
        internal const string TypeValue = "16";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("SUB")] public string Subscription { get; set; }
    }
}