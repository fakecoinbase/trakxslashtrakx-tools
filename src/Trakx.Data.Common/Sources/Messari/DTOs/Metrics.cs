using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public partial class Metrics
    {
        [JsonPropertyName("market_data")]
        public MarketData MarketData { get; set; }

        [JsonPropertyName("marketcap")]
        public Marketcap Marketcap { get; set; }

        [JsonPropertyName("supply")]
        public Supply Supply { get; set; }

        [JsonPropertyName("blockchain_stats_24_hours")]
        public Dictionary<string, double?> BlockchainStats24_Hours { get; set; }

        [JsonPropertyName("all_time_high")]
        public AllTimeHigh AllTimeHigh { get; set; }

        [JsonPropertyName("cycle_low")]
        public CycleLow CycleLow { get; set; }

        [JsonPropertyName("token_sale_stats")]
        public TokenSaleStats TokenSaleStats { get; set; }

        [JsonPropertyName("staking_stats")]
        public StakingStats StakingStats { get; set; }

        [JsonPropertyName("mining_stats")]
        public MiningStats MiningStats { get; set; }

        [JsonPropertyName("developer_activity")]
        public DeveloperActivity DeveloperActivity { get; set; }

        [JsonPropertyName("roi_data")]
        public Dictionary<string, double?> RoiData { get; set; }

        [JsonPropertyName("roi_by_year")]
        public Dictionary<string, double?> RoiByYear { get; set; }

        [JsonPropertyName("risk_metrics")]
        public RiskMetrics RiskMetrics { get; set; }

        [JsonPropertyName("misc_data")]
        public MiscData MiscData { get; set; }

        [JsonPropertyName("lend_rates")]
        public object LendRates { get; set; }

        [JsonPropertyName("borrow_rates")]
        public object BorrowRates { get; set; }

        [JsonPropertyName("loan_data")]
        public LoanData LoanData { get; set; }
    }
}