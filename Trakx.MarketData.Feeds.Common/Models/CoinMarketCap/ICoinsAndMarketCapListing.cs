namespace Trakx.MarketData.Feeds.Common.Models.CoinMarketCap
{
    public interface ICoinsAndMarketCapListing
    {
        ICoinAndMarketCap[] CoinsAndMarketCaps { get; set; }
        IStatus Status { get; set; }
    }
}