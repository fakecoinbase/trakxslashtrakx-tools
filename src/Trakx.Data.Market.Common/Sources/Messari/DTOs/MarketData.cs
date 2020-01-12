using System;
using System.Text.Json.Serialization;
using Trakx.Data.Market.Common.Serialisation.Converters;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class MarketData
    {
        [JsonPropertyName("price_usd")]
        public decimal? PriceUsd { get; set; }

        [JsonPropertyName("price_btc")]
        public decimal? PriceBtc { get; set; }

        [JsonPropertyName("volume_last_24_hours")]
        public decimal? VolumeLast24_Hours { get; set; }

        [JsonPropertyName("real_volume_last_24_hours")]
        public decimal? RealVolumeLast24_Hours { get; set; }

        [JsonPropertyName("volume_last_24_hours_overstatement_multiple")]
        public decimal? VolumeLast24_HoursOverstatementMultiple { get; set; }

        [JsonPropertyName("percent_change_usd_last_24_hours")]
        public double? PercentChangeUsdLast24_Hours { get; set; }

        [JsonPropertyName("percent_change_btc_last_24_hours")]
        public double? PercentChangeBtcLast24_Hours { get; set; }

        [JsonPropertyName("ohlcv_last_1_hour")]
        public OhlcvLastHour OhlcvLast1_Hour { get; set; }

        [JsonPropertyName("ohlcv_last_24_hour")]
        public OhlcvLastHour OhlcvLast24_Hour { get; set; }

        [JsonPropertyName("last_trade_at")]
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset? LastTradeAt { get; set; }
    }
}