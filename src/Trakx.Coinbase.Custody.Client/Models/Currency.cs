using System.Text.Json.Serialization;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class Currency
    {
        #nullable disable
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        #nullable restore

        [JsonPropertyName("decimals")]
        public ushort Decimals { get; set; }
    }
}

