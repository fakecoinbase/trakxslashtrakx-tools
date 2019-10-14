using System.Collections.Generic;

namespace Trakx.MarketApi.DataSources.Kaiko.Assets
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
