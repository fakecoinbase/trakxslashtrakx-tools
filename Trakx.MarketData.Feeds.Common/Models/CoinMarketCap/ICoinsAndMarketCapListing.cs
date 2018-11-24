namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface ICoinsAndMarketCapListing
    {
        ICoinAndMarketCap[] CoinsAndMarketCaps { get; }
        IStatus Status { get; }
    }
}