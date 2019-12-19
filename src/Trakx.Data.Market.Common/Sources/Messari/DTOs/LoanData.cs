using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class LoanData
    {
        [JsonPropertyName("Originated")]
        public CollateralLiquidated Originated { get; set; }

        [JsonPropertyName("Outstanding")]
        public CollateralLiquidated Outstanding { get; set; }

        [JsonPropertyName("Repaid")]
        public CollateralLiquidated Repaid { get; set; }

        [JsonPropertyName("Collateralized")]
        public CollateralLiquidated Collateralized { get; set; }

        [JsonPropertyName("CollateralLiquidated")]
        public CollateralLiquidated CollateralLiquidated { get; set; }

        [JsonPropertyName("CollateralRatio")]
        public Dictionary<string, long> CollateralRatio { get; set; }
    }
}