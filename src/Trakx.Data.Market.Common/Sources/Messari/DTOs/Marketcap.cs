using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class Marketcap
    {
        [JsonPropertyName("current_marketcap_usd")]
        public double CurrentMarketcapUsd { get; set; }

        [JsonPropertyName("y_2050_marketcap_usd")]
        public double? Y2050_MarketcapUsd { get; set; }

        [JsonPropertyName("y_plus10_marketcap_usd")]
        public double? YPlus10MarketcapUsd { get; set; }

        [JsonPropertyName("liquid_marketcap_usd")]
        public double? LiquidMarketcapUsd { get; set; }

        [JsonPropertyName("volume_turnover_last_24_hours_percent")]
        public double? VolumeTurnoverLast24_HoursPercent { get; set; }
    }
}