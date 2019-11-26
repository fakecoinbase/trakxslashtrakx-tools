using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Kaiko.DTOs
{
    public partial class ExchangesResponse   
    {
        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("data")]
        public List<Exchange> Exchanges { get; set; }
    }

    public partial class Exchange
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("kaiko_legacy_slug")]
        public string KaikoLegacySlug { get; set; }
    }

}
