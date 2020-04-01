using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs.Inbound
{
    public class SubscribeComplete : InboundMessageBase
    {
        public const string TypeValue = "16";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("SUB")] public string Subscription { get; set; }
    }
}