using System.Text.Json.Serialization;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class Currency
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("decimals")]
        public long Decimals { get; set; }
    }
}

