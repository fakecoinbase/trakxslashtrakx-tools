using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Kaiko.DTOs
{
    public partial class AssetsResponse
    {
        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("data")]
        public List<Asset> Assets { get; set; }
    }

    public partial class Asset
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("asset_class")]
        public AssetClass AssetClass { get; set; }

        [JsonPropertyName("asset_classes")]
        public List<AssetClass> AssetClasses { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AssetClass { Commodity, CryptoCurrency, Fiat };
}
