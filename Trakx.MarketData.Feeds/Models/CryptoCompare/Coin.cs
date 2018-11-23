using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;


namespace Trakx.MarketData.Feeds.Models.CryptoCompare
{
    class Coin : ICoin
    {
        public Coin(uint id, string url, string imageUrl, string name, string symbol, string coinName, string fullName, string algorithm, string proofType, string fullyPremined, decimal? totalCoinSupply, uint? builtOn, string smartContractAddress, string preMinedValue, string totalCoinsFreeFloat, uint sortOrder, bool sponsored)
        {
            Id = id;
            Url = url;
            ImageUrl = imageUrl;
            Name = name;
            Symbol = symbol;
            CoinName = coinName;
            FullName = fullName;
            Algorithm = algorithm;
            ProofType = proofType;
            FullyPremined = fullyPremined;
            TotalCoinSupply = totalCoinSupply;
            BuiltOn = builtOn;
            SmartContractAddress = smartContractAddress;
            PreMinedValue = preMinedValue;
            TotalCoinsFreeFloat = totalCoinsFreeFloat;
            SortOrder = sortOrder;
            Sponsored = sponsored;
        }

        /// <inheritdoc />
        public uint Id { get; }

        /// <inheritdoc />
        public string Url { get; }

        /// <inheritdoc />
        public string ImageUrl { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Symbol { get; }

        /// <inheritdoc />
        public string CoinName { get; }

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public string Algorithm { get; }

        /// <inheritdoc />
        public string ProofType { get; }

        /// <inheritdoc />
        public string FullyPremined { get; }

        /// <inheritdoc />
        public decimal? TotalCoinSupply { get; }

        /// <inheritdoc />
        public uint? BuiltOn { get; }

        /// <inheritdoc />
        public string SmartContractAddress { get; }

        /// <inheritdoc />
        public string PreMinedValue { get; }

        /// <inheritdoc />
        public string TotalCoinsFreeFloat { get; }

        /// <inheritdoc />
        public uint SortOrder { get; }

        /// <inheritdoc />
        public bool Sponsored { get; }
    }
}
