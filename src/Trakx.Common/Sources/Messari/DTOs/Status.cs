using System;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public partial class Status
    {
        [JsonPropertyName("elapsed")]
        public long? Elapsed { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }
}