using System.Collections.Generic;

namespace Trakx.MarketApi.DataSources.Kaiko.DTOs
{
    public partial class AssetsResponse
    {
        public string Result { get; set; }
        public List<Asset> Data { get; set; }
    }

    public partial class Asset
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string AssetClass { get; set; }
    }
}
