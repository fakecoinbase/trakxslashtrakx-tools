using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Kaiko.DTOs
{
    public partial class Access
    {
        [JsonPropertyName("access_range")]
        public Range AccessRange { get; set; }

        [JsonPropertyName("data_range")]
        public Range DataRange { get; set; }
    }
}