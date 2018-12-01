using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;
using Trakx.MarketData.Feeds.Common.Models.Trakx;

namespace Trakx.MarketData.Feeds.Models.Trakx
{
    public class Coin : Models.CryptoCompare.Coin, ICryptoCompareCoinAndMarketCap
    {
        public decimal UsdMarketCap { get; }

        /// <inheritdoc />
        public Coin(
            long id,
            string url,
            string imageUrl,
            string name,
            string symbol,
            string coinName,
            string fullName,
            string algorithm,
            string proofType,
            long fullyPremined,
            string totalCoinSupply,
            long? builtOn,
            string smartContractAddress,
            decimal? preMinedValue,
            ulong? totalCoinsFreeFloat,
            long sortOrder,
            bool sponsored,
            bool isTrading,
            decimal usdMarketCap)
            : base(
                id,
                url,
                imageUrl,
                name,
                symbol,
                coinName,
                fullName,
                algorithm,
                proofType,
                fullyPremined,
                totalCoinSupply,
                builtOn,
                smartContractAddress,
                preMinedValue,
                totalCoinsFreeFloat,
                sortOrder,
                sponsored,
                isTrading)
        {
            UsdMarketCap = usdMarketCap;
        }

        public Coin(ICoin cryptoCompareCoin, decimal usdMarketCap)
        : this(cryptoCompareCoin.Id,
            cryptoCompareCoin.Url,
            cryptoCompareCoin.ImageUrl,
            cryptoCompareCoin.Name,
            cryptoCompareCoin.Symbol,
            cryptoCompareCoin.CoinName,
            cryptoCompareCoin.FullName,
            cryptoCompareCoin.Algorithm,
            cryptoCompareCoin.ProofType,
            cryptoCompareCoin.FullyPremined,
            cryptoCompareCoin.TotalCoinSupply,
            cryptoCompareCoin.BuiltOn,
            cryptoCompareCoin.SmartContractAddress,
            cryptoCompareCoin.PreMinedValue,
            cryptoCompareCoin.TotalCoinsFreeFloat,
            cryptoCompareCoin.SortOrder,
            cryptoCompareCoin.Sponsored,
            cryptoCompareCoin.IsTrading,
            usdMarketCap)
        {

        }
    }
}
