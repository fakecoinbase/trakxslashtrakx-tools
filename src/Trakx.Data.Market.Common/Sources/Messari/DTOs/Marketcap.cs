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