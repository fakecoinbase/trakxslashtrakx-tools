using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class SharpeRatios
    {
        [JsonPropertyName("last_30_days")]
        public double? Last30_Days { get; set; }

        [JsonPropertyName("last_90_days")]
        public double? Last90_Days { get; set; }

        [JsonPropertyName("last_1_year")]
        public double? Last1_Year { get; set; }

        [JsonPropertyName("last_3_years")]
        public double? Last3_Years { get; set; }
    }
}