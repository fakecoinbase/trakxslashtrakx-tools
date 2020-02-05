using System;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
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