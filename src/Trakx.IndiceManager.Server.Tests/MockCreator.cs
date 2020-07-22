using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.IndiceManager.Server.Models;
using Xunit.Abstractions;
using MockCreatorCoinbase = Trakx.Coinbase.Custody.Client.Tests.MockCreator;

namespace Trakx.IndiceManager.Server.Tests
{
    public class MockCreator : Trakx.Common.Tests.MockCreator
    {
        private readonly MockCreatorCoinbase _mockCreatorCoinbase;

        /// <inheritdoc />
        public MockCreator(ITestOutputHelper output) : base(output)
        {
            _mockCreatorCoinbase = new MockCreatorCoinbase(output);
        }

        public AccountBalanceModel GetRandomAccountBalanceModel()
        {
            var nativeBalance = GetRandomUnscaledAmount();
            var decimals = GetRandomDecimals();
            var balance = nativeBalance / (decimal)Math.Pow(10, decimals);
            var symbol = GetRandomString(3);
            var name = $"account name {GetRandomString(5)}";
            var lastUpdate = GetRandomUtcDateTimeOffset();
            var address = GetRandomAddressEthereum();
            var accountBalanceModel =
                new AccountBalanceModel(symbol, balance, nativeBalance, name, address, lastUpdate);
            return accountBalanceModel;
        }

        public IndiceDetailModel GetRandomIndexDetailModel()
        {
            var indexDefinition = GetRandomIndiceDefinition();
            var indexDetails = new IndiceDetailModel
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
            return indexDetails;
        }

        public IndiceCompositionModel GetRandomIndiceCompositionModel()
        {
            var composition = GetIndiceComposition(3);
            var compositonModel = new IndiceCompositionModel(composition);
            return compositonModel;
        }

        public Currency GetRandomCurrency(string symbol) => _mockCreatorCoinbase.GetRandomCurrency(symbol);

        public Wallet GetRandomWallet() => _mockCreatorCoinbase.GetRandomWallet();
    }
}
