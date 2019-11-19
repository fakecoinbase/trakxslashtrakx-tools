#region LICENSE

// 
// Copyright (c) 2019 Catalyst Network
// 
// This file is part of Catalyst.Node <https://github.com/catalyst-network/Catalyst.Node>
// 
// Catalyst.Node is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Catalyst.Node is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Catalyst.Node. If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
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