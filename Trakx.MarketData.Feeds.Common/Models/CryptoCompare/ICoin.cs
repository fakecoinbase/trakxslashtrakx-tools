using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.Converters;

namespace Trakx.MarketData.Feeds.Common.Models.CryptoCompare
{
    public interface ICoin
    {
        uint Id { get; }
        string Url { get; }
        string ImageUrl { get; }
        string Name { get; }
        string Symbol { get; }
        string CoinName { get; }
        string FullName { get; }
        string Algorithm { get; }
        string ProofType { get; }
        string FullyPremined { get; }
        [JsonConverter(typeof(DecimalWithNaConverter))]
        decimal? TotalCoinSupply { get; }
        [JsonConverter(typeof(Uint32WithNaConverter))]
        uint? BuiltOn { get; }
        string SmartContractAddress { get; }
        string PreMinedValue { get; }
        string TotalCoinsFreeFloat { get; }
        uint SortOrder { get; }
        bool Sponsored { get; }
    }
}
