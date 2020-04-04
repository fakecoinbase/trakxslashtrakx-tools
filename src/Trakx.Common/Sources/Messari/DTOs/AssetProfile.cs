using System;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
{
    public class AssetProfile : Profile
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }
}