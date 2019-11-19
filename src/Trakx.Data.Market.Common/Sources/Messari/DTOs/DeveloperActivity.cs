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
    public partial class DeveloperActivity
    {
        [JsonPropertyName("stars")]
        public long? Stars { get; set; }

        [JsonPropertyName("watchers")]
        public long? Watchers { get; set; }

        [JsonPropertyName("commits_last_3_months")]
        public long? CommitsLast3_Months { get; set; }

        [JsonPropertyName("commits_last_1_year")]
        public long? CommitsLast1_Year { get; set; }

        [JsonPropertyName("lines_added_last_3_months")]
        public long? LinesAddedLast3_Months { get; set; }

        [JsonPropertyName("lines_added_last_1_year")]
        public long? LinesAddedLast1_Year { get; set; }

        [JsonPropertyName("lines_deleted_last_3_months")]
        public long? LinesDeletedLast3_Months { get; set; }

        [JsonPropertyName("lines_deleted_last_1_year")]
        public long? LinesDeletedLast1_Year { get; set; }
    }
}