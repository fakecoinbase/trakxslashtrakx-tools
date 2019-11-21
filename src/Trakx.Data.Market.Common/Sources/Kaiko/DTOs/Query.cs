using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Kaiko.DTOs
{
    public partial class Query
    {
        [JsonPropertyName("data_version")]
        public string DataVersion { get; set; }

        [JsonPropertyName("commodity")]
        public string Commodity { get; set; }

        [JsonPropertyName("start_time")]
        public DateTimeOffset StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public DateTimeOffset EndTime { get; set; }

        [JsonPropertyName("page_size")]
        public long PageSize { get; set; }

        [JsonPropertyName("interval")]
        public string Interval { get; set; }

        [JsonPropertyName("quote_asset")]
        public string QuoteAsset { get; set; }

        [JsonPropertyName("base_asset")]
        public string BaseAsset { get; set; }

        [JsonPropertyName("request_time")]
        public DateTimeOffset RequestTime { get; set; }

        [JsonPropertyName("instruments")]
        public List<string> Instruments { get; set; }

        [JsonPropertyName("start_timestamp")]
        public long StartTimestamp { get; set; }

        [JsonPropertyName("end_timestamp")]
        public long EndTimestamp { get; set; }
    }
}
