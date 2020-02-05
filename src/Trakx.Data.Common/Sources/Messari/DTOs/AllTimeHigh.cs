using System;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public partial class AllTimeHigh
    {
        [JsonPropertyName("price")]
        public double? Price { get; set; }

        [JsonPropertyName("at")]
        public DateTimeOffset? At { get; set; }

        [JsonPropertyName("days_since")]
        public long? DaysSince { get; set; }

        [JsonPropertyName("percent_down")]
        public double? PercentDown { get; set; }

        [JsonPropertyName("breakeven_multiple")]
        public double? BreakevenMultiple { get; set; }
    }
}