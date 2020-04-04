using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.Messari.DTOs
{
    public partial class People
    {
        [JsonPropertyName("founding_team")]
        public object FoundingTeam { get; set; }

        [JsonIgnore]
        [JsonPropertyName("contributors")]
        public List<Advisor> Contributors { get; set; }

        [JsonIgnore]
        [JsonPropertyName("investors")]
        public List<Advisor> Investors { get; set; }

        [JsonIgnore]
        [JsonPropertyName("advisors")]
        public List<Advisor> Advisors { get; set; }
    }
}