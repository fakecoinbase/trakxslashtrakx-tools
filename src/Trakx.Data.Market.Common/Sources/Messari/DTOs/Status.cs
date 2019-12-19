using System;
using System.Text.Json.Serialization;
using Trakx.Data.Market.Common.Serialisation.Converters;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class Status
    {
        [JsonPropertyName("elapsed")]
        [JsonConverter(typeof(StringLongConverter))]
        public long Elapsed { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }
}