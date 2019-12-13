using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Kaiko.DTOs
{
    public partial class Range
    {
        [JsonPropertyName("start_timestamp")]
        public object StartTimestamp { get; set; }

        [JsonPropertyName("end_timestamp")]
        public object EndTimestamp { get; set; }
    }
}