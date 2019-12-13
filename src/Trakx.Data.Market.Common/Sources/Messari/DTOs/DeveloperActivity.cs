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