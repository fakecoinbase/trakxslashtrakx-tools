using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.CryptoCompare.DTOs.Inbound
{
    public class Error : InboundMessageBase
    {
        internal const string TypeValue = "500";
        [JsonPropertyName("MESSAGE")] public string Message { get; set; }
        [JsonPropertyName("PARAMETER")] public string Parameter { get; set; }
        [JsonPropertyName("INFO")] public string Info { get; set; }
    }
}