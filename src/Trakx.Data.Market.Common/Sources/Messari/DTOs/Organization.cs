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
    public partial class Organization
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("founded_date")]
        public DateTimeOffset? FoundedDate { get; set; }

        [JsonPropertyName("governance")]
        public string? Governance { get; set; }

        //[JsonPropertyName("legal_structure")]
        [JsonIgnore]
        public LegalStructure? LegalStructure { get; set; }

        [JsonPropertyName("jurisdiction")]
        public string Jurisdiction { get; set; }

        [JsonPropertyName("org_charter")]
        public object OrgCharter { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("people_count_estimate")]
        public string PeopleCountEstimate { get; set; }
    }
}