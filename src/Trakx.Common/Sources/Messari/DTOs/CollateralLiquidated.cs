using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.Messari.DTOs
{
    public partial class CollateralLiquidated
    {
        [JsonPropertyName("CompoundV2")]
        public long CompoundV2 { get; set; }

        [JsonPropertyName("DyDx2")]
        public long DyDx2 { get; set; }

        [JsonPropertyName("MakerDAO")]
        public long MakerDao { get; set; }
    }
}