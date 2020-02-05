using System;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public partial class CycleLow
    {
        [JsonPropertyName("price")]
        public double? Price { get; set; }

        [JsonPropertyName("at")]
        public DateTimeOffset? At { get; set; }

        [JsonPropertyName("percent_up")]
        public double? PercentUp { get; set; }

        [JsonPropertyName("days_since")]
        public long? DaysSince { get; set; }
    }
}