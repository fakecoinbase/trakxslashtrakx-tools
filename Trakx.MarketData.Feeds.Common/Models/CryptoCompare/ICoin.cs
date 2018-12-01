namespace Trakx.MarketData.Feeds.Common.Models.CryptoCompare
{
    public interface ICoin
    {
        long Id { get; }
        string Url { get; }
        string ImageUrl { get; }
        string Name { get; }
        string Symbol { get; }
        string CoinName { get; }
        string FullName { get; }
        string Algorithm { get; }
        string ProofType { get; }
        long FullyPremined { get; }
        string TotalCoinSupply { get; }
        long? BuiltOn { get; }
        string SmartContractAddress { get; }
        decimal? PreMinedValue { get; }
        ulong? TotalCoinsFreeFloat { get; }
        long SortOrder { get; }
        bool Sponsored { get; }
        bool IsTrading { get; }
    }
}