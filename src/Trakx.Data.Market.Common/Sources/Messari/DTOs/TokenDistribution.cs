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
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class TokenDistribution
    {
        [JsonPropertyName("sale_start")]
        public DateTimeOffset? SaleStart { get; set; }

        [JsonPropertyName("sale_end")]
        public DateTimeOffset? SaleEnd { get; set; }

        [JsonPropertyName("initial_distribution")]
        public long? InitialDistribution { get; set; }

        [JsonPropertyName("current_supply")]
        public long? CurrentSupply { get; set; }

        [JsonPropertyName("max_supply")]
        public long? MaxSupply { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}