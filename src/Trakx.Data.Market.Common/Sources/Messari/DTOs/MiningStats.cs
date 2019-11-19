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
    public partial class MiningStats
    {
        [JsonPropertyName("mining_algo")]
        public string MiningAlgo { get; set; }

        [JsonPropertyName("network_hash_rate")]
        public string NetworkHashRate { get; set; }

        [JsonPropertyName("available_on_nicehash_percent")]
        public double? AvailableOnNicehashPercent { get; set; }

        [JsonPropertyName("1_hour_attack_cost")]
        public double? The1_HourAttackCost { get; set; }

        [JsonPropertyName("24_hours_attack_cost")]
        public double? The24_HoursAttackCost { get; set; }

        [JsonPropertyName("attack_appeal")]
        public double? AttackAppeal { get; set; }
    }
}