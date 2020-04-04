﻿using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.Messari.DTOs
{
    public partial class GetAssetMetricsResponse
    {
        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonPropertyName("data")]
        public AssetMetrics Data { get; set; }
    }
}