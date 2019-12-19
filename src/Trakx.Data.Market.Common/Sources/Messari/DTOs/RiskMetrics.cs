using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class RiskMetrics
    {
        [JsonPropertyName("sharpe_ratios")]
        public SharpeRatios SharpeRatios { get; set; }

        [JsonPropertyName("volatility_stats")]
        public VolatilityStats VolatilityStats { get; set; }
    }
}