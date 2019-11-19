﻿#region LICENSE

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
    public partial class VolatilityStats
    {
        [JsonPropertyName("volatility_last_30_days")]
        public double? VolatilityLast30_Days { get; set; }

        [JsonPropertyName("volatility_last_90_days")]
        public double? VolatilityLast90_Days { get; set; }

        [JsonPropertyName("volatility_last_1_year")]
        public double? VolatilityLast1_Year { get; set; }

        [JsonPropertyName("volatility_last_3_years")]
        public double? VolatilityLast3_Years { get; set; }
    }
}