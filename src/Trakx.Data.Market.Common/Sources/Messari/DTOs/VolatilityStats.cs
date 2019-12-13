using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class VolatilityStats
    {
        [JsonPropertyName("volatility_last_30_days")]
        public double? VolatilityLast30_Days { get; set; }

        [JsonPropertyName("volatility_last_90_days")]
        public double? VolatilityLast90_Days { get; set; }

        [JsonPropertyName("volatility_last_1_year")]
        public double? VolatilityLast1_Year { get; set; }

        [JsonPropertyName("volatility_last_3_years")]
        public double? VolatilityLast3_Years { get; set; }
    }
}