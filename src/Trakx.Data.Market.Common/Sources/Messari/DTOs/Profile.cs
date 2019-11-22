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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class Profile
    {
        [JsonPropertyName("is_verified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("tagline")]
        public string Tagline { get; set; }

        [JsonIgnore]
        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonIgnore]
        [JsonPropertyName("background")]
        public string Background { get; set; }

        [JsonIgnore]
        [JsonPropertyName("technology")]
        public string Technology { get; set; }

        [JsonPropertyName("category")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProfileCategory Category { get; set; }

        [JsonPropertyName("sector")]
        public string Sector { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("sfarScore")]
        public double? SfarScore { get; set; }

        [JsonPropertyName("token_distribution")]
        public TokenDistribution TokenDistribution { get; set; }

        [JsonPropertyName("token_details")]
        public TokenDetails TokenDetails { get; set; }

        [JsonPropertyName("organizations")]
        public List<Organization> Organizations { get; set; }

        [JsonPropertyName("people")]
        public People People { get; set; }

        [JsonPropertyName("relevant_resources")]
        public List<RelevantResource> RelevantResources { get; set; }

        [JsonPropertyName("consensus_algorithm")]
        public string ConsensusAlgorithm { get; set; }
    }
}