using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public class Profile
    {
        [JsonPropertyName("is_verified")]
        public bool? IsVerified { get; set; }

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
        //[JsonConverter(typeof(StringNullableEnumConverter<ProfileCategory?>))]
        public string? Category { get; set; }

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