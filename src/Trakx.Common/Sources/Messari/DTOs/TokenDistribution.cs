using System;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public partial class TokenDistribution
    {
        [JsonPropertyName("sale_start")]
        public DateTimeOffset? SaleStart { get; set; }

        [JsonPropertyName("sale_end")]
        public DateTimeOffset? SaleEnd { get; set; }

        [JsonIgnore]
        [JsonPropertyName("initial_distribution")]
        public long? InitialDistribution { get; set; }

        [JsonPropertyName("current_supply")]
        public decimal? CurrentSupply { get; set; }

        [JsonPropertyName("max_supply")]
        public decimal? MaxSupply { get; set; }

        [JsonIgnore]
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}