using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Models.Trakx
{

    public interface ICryptoCompareCoinAndMarketCap : ICoin
    {
        decimal UsdMarketCap { get; }
    }
}
