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
    public partial class TokenDetails
    {
        [JsonPropertyName("usage")]
        public string Usage { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("sales_rounds")]
        public List<SalesRound> SalesRounds { get; set; }

        [JsonPropertyName("block_reward")]
        public double? BlockReward { get; set; }

        [JsonPropertyName("targeted_block_time_in_sec")]
        public double? TargetedBlockTimeInSec { get; set; }

        [JsonPropertyName("on_chain_governance_structure")]
        public string OnChainGovernanceStructure { get; set; }

        [JsonPropertyName("is_treasury_decentralized")]
        public bool? IsTreasuryDecentralized { get; set; }

        [JsonPropertyName("launch_style")]
        public string LaunchStyle { get; set; }

        [JsonPropertyName("initial_supply")]
        public double? InitialSupply { get; set; }

        [JsonPropertyName("percentage_allocated_to_investors_from_initial_supply")]
        public double? PercentageAllocatedToInvestorsFromInitialSupply { get; set; }

        [JsonPropertyName("percentage_allocated_to_premined_or_airdrops_from_initial_supply")]
        public double? PercentageAllocatedToPreminedOrAirdropsFromInitialSupply { get; set; }

        [JsonPropertyName("percentage_allocated_to_organizations_or_founders_supply")]
        public double? PercentageAllocatedToOrganizationsOrFoundersSupply { get; set; }

        [JsonPropertyName("mining_algorithm")]
        public string MiningAlgorithm { get; set; }

        [JsonPropertyName("next_halving_date")]
        public DateTimeOffset? NextHalvingDate { get; set; }

        [JsonPropertyName("genesis_block_date")]
        public DateTimeOffset? GenesisBlockDate { get; set; }

        [JsonPropertyName("is_victim_of_51_percent_attack")]
        public bool? IsVictimOf51_PercentAttack { get; set; }

        [JsonIgnore]
        //[JsonPropertyName("emission_type_general")] 
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public EmissionTypeGeneral EmissionTypeGeneral { get; set; }

        [JsonPropertyName("emission_type_precise")]
        public string EmissionTypePrecise { get; set; }

        [JsonPropertyName("is_capped_supply")]
        public bool? IsCappedSupply { get; set; }

        [JsonPropertyName("max_supply")]
        public long? MaxSupply { get; set; }
    }
}