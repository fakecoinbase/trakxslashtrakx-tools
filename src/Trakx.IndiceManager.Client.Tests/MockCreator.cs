using System;
using System.Collections.Generic;
using Trakx.Common.Interfaces.Indice;
using Trakx.IndiceManager.ApiClient;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Client.Tests
{
    public class MockCreator : Trakx.Common.Tests.MockCreator
    {
        /// <inheritdoc />
        public MockCreator(ITestOutputHelper output) : base(output) {}

        public ComponentDetailModel GetRandomComponentDetailModel()
        {
            var quantity = GetComponentQuantity();
            var componentDetailModel = new ComponentDetailModel
            {
                Address = quantity.ComponentDefinition.Address,
                CoinGeckoId = quantity.ComponentDefinition.CoinGeckoId,
                Decimals = quantity.ComponentDefinition.Decimals,
                Quantity = quantity.Quantity,
                Name = quantity.ComponentDefinition.Name,
                Symbol = quantity.ComponentDefinition.Symbol,
                UsdcValue = GetRandomPrice() * 1000,
                Weight = (decimal) Random.NextDouble()
            };
            return componentDetailModel;
        }

        public IndiceDetailModel GetRandomIndiceDetailModel(IIndiceDefinition? indexDefinition = default)
        {
            indexDefinition ??= GetRandomIndiceDefinition();
            var indexDetailModel = new IndiceDetailModel
            {
                Address = indexDefinition.Address,
                CreationDate = indexDefinition.CreationDate,
                Description = indexDefinition.Description,
                IndiceCompositions = new List<IndiceCompositionModel>(),
                IndiceState = "Published",
                Name = indexDefinition.Name,
                NaturalUnit = indexDefinition.NaturalUnit,
                Symbol = indexDefinition.Symbol
            };
            return indexDetailModel;
        }

        public IndiceCompositionModel GetRandomIndiceCompositionModel(int componentCount = 3)
        {
            var composition = GetIndiceComposition(componentCount);
            var indexCompositionModel = new IndiceCompositionModel
            {
                Address = composition.Address,
                CreationDate = composition.CreationDate,
                IndiceDetail = GetRandomIndiceDetailModel(composition.IndiceDefinition),
                Symbol = composition.Symbol,
                TargetedNav = GetRandomValue(),
                Version = (int)GetRandomCompositionVersion()
            };
            return indexCompositionModel;
        }

        public AccountBalanceModel GetRandomAccountBalanceModel()
        {
            var nativeBalance = (long)GetRandomUnscaledAmount();
            var decimals = GetRandomDecimals();
            var balance = nativeBalance / (decimal) Math.Pow(10, decimals);
            var accountBalanceModel = new AccountBalanceModel
            {
                Address = GetRandomAddressEthereum(),
                Balance = balance,
                LastUpDate = GetRandomUtcDateTimeOffset(),
                Name = $"account name {GetRandomString(5)}",
                NativeBalance = nativeBalance,
                Symbol = GetRandomString(3)
            };
            return accountBalanceModel;
        }
    }
}