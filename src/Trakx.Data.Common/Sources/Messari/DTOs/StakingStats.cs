using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public partial class StakingStats
    {
        [JsonPropertyName("staking_yield_percent")]
        public double? StakingYieldPercent { get; set; }

        [JsonPropertyName("staking_type")]
        public string StakingType { get; set; }

        [JsonPropertyName("staking_minimum")]
        public object StakingMinimum { get; set; }

        [JsonPropertyName("tokens_staked")]
        public double? TokensStaked { get; set; }

        [JsonPropertyName("tokens_staked_percent")]
        public double? TokensStakedPercent { get; set; }

        [JsonPropertyName("real_staking_yield_percent")]
        public double? RealStakingYieldPercent { get; set; }
    }
}