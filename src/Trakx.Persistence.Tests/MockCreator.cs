using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Transaction;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests
{
    public static class TestOutputHelperExtensions
    {
        public static string GetCurrentTestName(this ITestOutputHelper output)
        {
            var currentTest = output
                .GetType()
                .GetField("test", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(output) as ITest;
            if (currentTest == null)
            {
                throw new ArgumentNullException(
                    $"Failed to reflect current test as {nameof(ITest)} from {nameof(output)}");
            }

            var currentTestName = currentTest.TestCase.TestMethod.Method.Name;
            return currentTestName;
        }
    }

    public class MockCreator
    {
        private readonly Random _random;
        private readonly string _name;
        private const string AddressChars = "abcdef01234566789";
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        public MockCreator(ITestOutputHelper output)
        {
            _name = output.GetCurrentTestName();
            _random = new Random(_name.GetHashCode());
        }

        public string GetRandomAddressEthereum() => "0x" + new string(Enumerable.Range(0, 40)
            .Select(_ => AddressChars[_random.Next(0, AddressChars.Length)]).ToArray());
        public string GetRandomEthereumTransactionHash() => "0x" + new string(Enumerable.Range(0, 64)
                                                        .Select(_ => AddressChars[_random.Next(0, AddressChars.Length)]).ToArray());

        public string GetRandomString(int size) => new string(Enumerable.Range(0, size)
            .Select(_ => Alphabet[_random.Next(0, Alphabet.Length)]).ToArray());

        public string GetRandomIndiceSymbol(string? indiceShortName = default) => (_random.Next(1) < 1 ? "l" : "s")
                                                                      + _random.Next(1, 20)
                                                                      + (indiceShortName ?? GetRandomString(3));

        public string GetRandomCompositionSymbol(string? indiceShortName = default)
            => GetRandomIndiceSymbol(indiceShortName) + GetRandomYearMonthSuffix();

        public string GetRandomYearMonthSuffix() => $"{_random.Next(20, 36):00}{_random.Next(1, 13):00}";

        public DateTime GetRandomDateTime()
        {
            var firstJan2020 = new DateTime(2020, 01, 01);
            var firstJan2050 = new DateTime(2050, 01, 01);
            var timeBetween2020And2050 = firstJan2050.Subtract(firstJan2020);

            var randomDay = firstJan2020 + TimeSpan.FromDays(_random.Next(0, (int)timeBetween2020And2050.TotalDays));
            return randomDay;
        }

        public ushort GetRandomNaturalUnit() => (ushort)_random.Next(0, 18);
        public uint GetRandomCompositionVersion() => (uint)_random.Next(0, 30);
        public decimal GetRandomPrice() => _random.Next(1, int.MaxValue)/1e5m;
        public long GetRandomUnscaledAmount() => _random.Next(1, int.MaxValue);
        public TimeSpan GetRandomTimeSpan() => TimeSpan.FromSeconds(_random.Next(1, (int)TimeSpan.FromDays(1000).TotalSeconds));

        public IIndiceComposition GetIndiceComposition(int componentCount)
        {
            var componentQuantities = Enumerable.Range(0, componentCount)
                .Select(i => GetComponentQuantity(symbol: $"sym{i}", coinGeckoId: $"id-{i}"))
                .ToList();

            var indiceComposition = Substitute.For<IIndiceComposition>();
            indiceComposition.ComponentQuantities.Returns(componentQuantities);
            var randomIndiceDefinition = GetRandomIndiceDefinition();
            indiceComposition.IndiceDefinition.Returns(randomIndiceDefinition);
            indiceComposition.Address.Returns(GetRandomAddressEthereum());
            var creationDate = GetRandomDateTime();
            indiceComposition.CreationDate.Returns(creationDate);
            var compositionSymbol = randomIndiceDefinition.GetCompositionSymbol(creationDate);
            indiceComposition.Symbol.Returns(compositionSymbol);
            return indiceComposition;
        }

        public IWrappingTransaction GetWrappingTransaction(TransactionState transactionState = TransactionState.Complete)
        {
            var transaction = Substitute.For<IWrappingTransaction>();
            transaction.SenderAddress.Returns(GetRandomAddressEthereum());
            transaction.User.Returns(_name);
            transaction.ToCurrency.Returns(GetRandomCompositionSymbol());
            transaction.FromCurrency.Returns(GetRandomCompositionSymbol());
            transaction.Amount.Returns(10.03m);
            transaction.NativeChainTransactionHash.Returns(GetRandomAddressEthereum());
            transaction.ReceiverAddress.Returns(GetRandomAddressEthereum());
            transaction.NativeChainTransactionHash.Returns(GetRandomAddressEthereum());
            transaction.EthereumTransactionHash.Returns(GetRandomEthereumTransactionHash());
            transaction.TimeStamp.Returns(GetRandomDateTime());
            transaction.TransactionState.Returns(transactionState);
            transaction.TransactionType.Returns(TransactionType.Wrap);
            if (transactionState != TransactionState.Complete) return transaction;

            transaction.NativeChainBlockId.Returns(_random.Next(600000));
            transaction.EthereumBlockId.Returns(_random.Next(800000));
            return transaction;
        }

        public IIndiceDefinition GetRandomIndiceDefinition(string? indiceSymbol = default, string? name = default)
        {
            var indiceDefinition = Substitute.For<IIndiceDefinition>();
            indiceDefinition.NaturalUnit.Returns((ushort)10);
            indiceSymbol ??= GetRandomIndiceSymbol();
            indiceDefinition.Symbol.Returns(indiceSymbol);
            name ??= "indice name " + GetRandomString(15);
            indiceDefinition.Name.Returns(name);
            var description = "description " + GetRandomString(15);
            indiceDefinition.Description.Returns(description);
            indiceDefinition.Address.Returns(GetRandomAddressEthereum());
            indiceDefinition.CreationDate.Returns(GetRandomDateTime());
            return indiceDefinition;
        }

        public IComponentQuantity GetComponentQuantity(string? address = default,
            string? symbol = default,
            string? name = default,
            string? coinGeckoId = default,
            decimal? quantity = default,
            ushort? decimals = default)
        {
            var componentQuantity = Substitute.For<IComponentQuantity>();
            quantity ??= (decimal)(_random.NextDouble() + 0.001 * 10000);
            componentQuantity.Quantity.Returns(quantity.Value);

            var definition = GetRandomComponentDefinition(address, symbol, name, coinGeckoId, decimals);
            componentQuantity.ComponentDefinition.Returns(definition);

            return componentQuantity;
        }

        public IComponentDefinition GetRandomComponentDefinition(string? address = default,
            string? symbol = default,
            string? name = default,
            string? coinGeckoId = default,
            ushort? decimals = default)
        {
            var definition = Substitute.For<IComponentDefinition>();

            address ??= GetRandomAddressEthereum();
            definition.Address.Returns(address);
            decimals ??= (ushort)_random.Next(0, 19);
            definition.Decimals.Returns(decimals.Value);
            symbol ??= GetRandomString(3);
            definition.Symbol.Returns(symbol);
            name ??= "name " + GetRandomString(24);
            definition.Name.Returns(name);
            coinGeckoId ??= "id-" + GetRandomString(5);
            definition.CoinGeckoId.Returns(coinGeckoId);
            
            return definition;
        }

        public TransactionReceipt GetTransactionReceipt()
        {
            var transactionReceipt = new TransactionReceipt()
            {
                TransactionHash = "0x123456789",
                Logs = JArray.Parse(
                    "[\r\n" +
                    "  {\r\n" +
                    "    \"address\": \"0xf55186cc537e7067ea616f2aae007b4427a120c8\",\r\n" +
                    "    \"blockHash\": \"0x6c54a6c04c3971e4fb5e4a4c84ee25148ab776a15ff44ce8b79148e4a70ca4a9\",\r\n" +
                    "    \"blockNumber\": \"0x93edb6\",\r\n" +
                    "    \"data\": \"0x000000000000000000000000e1cd722575800055\",\r\n" +
                    "    \"logIndice\": \"0x46\",\r\n" +
                    "    \"removed\": false,\r\n" +
                    "    \"topics\": [\r\n" +
                    "      \"0xa31e381e140096a837a20ba16eb64e32a4011fda0697adbfd7a8f7341c56aa94\",\r\n" +
                    "      \"0x000000000000000000000000ae81ae0179b38588e05f404e05882a3965d1b415\"\r\n" +
                    "    ],\r\n" +
                    "    \"transactionHash\": \"0x2e39c249e929b8d2dcd2560bd33e1ebd17570742972866b46060bc42bf7c4052\",\r\n" +
                    "    \"transactionIndice\": \"0x80\"\r\n" +
                    "  }" +
                    "\r\n]")
            };
            return transactionReceipt;
        }

        public IIndiceValuation GetRandomIndexValuation(IIndiceComposition composition, string quoteCurrency = "usdc")
        {
            var componentValuations = composition.ComponentQuantities.Select(
                c =>
                {
                    var valuation = new ComponentValuation(c, quoteCurrency, GetRandomPrice(),"fakeSource", DateTime.Now);
                    return (IComponentValuation)valuation;
                }).ToList();

            var indiceValuation = new IndiceValuation(composition, componentValuations, DateTime.Now);
            return indiceValuation;
        }

        public IUser GetRandomUser()
        {
            var user = Substitute.For<IUser>();
            var id = GetRandomString(8);
            user.Id.Returns(id);
            var list = new List<IDepositorAddress>();
            user.Addresses.Returns(list);
            return user;
        }
    }
}