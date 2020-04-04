using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.CryptoCompare.DTOs.Inbound
{
    public class UnsubscribeComplete : InboundMessageBase
    {
        internal const string TypeValue = "17";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("PARAMETER")] public string Parameter { get; set; }
        [JsonPropertyName("INFO")] public string Info { get; set; }
    }
}