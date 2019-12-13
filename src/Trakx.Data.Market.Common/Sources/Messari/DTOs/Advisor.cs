using System;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class Advisor
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("twitter")]
        public Uri Twitter { get; set; }

        [JsonPropertyName("github")]
        public Uri Github { get; set; }

        [JsonPropertyName("medium")]
        public Uri Medium { get; set; }

        [JsonPropertyName("linkedin")]
        public Uri Linkedin { get; set; }
    }
}