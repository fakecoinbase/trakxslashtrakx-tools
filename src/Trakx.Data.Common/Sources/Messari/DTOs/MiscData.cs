using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public partial class MiscData
    {
        [JsonPropertyName("private_market_price_usd")]
        public object PrivateMarketPriceUsd { get; set; }

        [JsonPropertyName("vladimir_club_cost")]
        public double? VladimirClubCost { get; set; }

        [JsonPropertyName("btc_current_normalized_supply_price_usd")]
        public double? BtcCurrentNormalizedSupplyPriceUsd { get; set; }

        [JsonPropertyName("btc_y2050_normalized_supply_price_usd")]
        public double? BtcY2050NormalizedSupplyPriceUsd { get; set; }

        [JsonPropertyName("asset_created_at")]
        public DateTimeOffset? AssetCreatedAt { get; set; }

        [JsonPropertyName("asset_age_days")]
        public long? AssetAgeDays { get; set; }

        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; }

        [JsonPropertyName("sectors")]
        public List<string> Sectors { get; set; }
    }
}