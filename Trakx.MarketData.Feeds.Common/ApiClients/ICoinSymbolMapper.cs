namespace Trakx.MarketData.Feeds.Common.ApiClients
{
    public interface ICoinSymbolMapper
    {
        string CryptoCompareToCoinMarketCap(string cryptoCompareSymbol);

        string CoinMarketCapToCryptoCompare(string coinMarketCapSymbol);
    }
}