using System;
using System.Linq;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Tests.Data
{
    public class MockCreator
    {
        private static readonly Random Random = new Random();
        private const string AddressChars = "abcdef01234566789";
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        public string GetRandomAddressEthereum() => "0x" + new string(Enumerable.Range(0, 40)
            .Select(_ => AddressChars[Random.Next(0, AddressChars.Length)]).ToArray());

        public string GetRandomString(int size) => new string(Enumerable.Range(0, size)
            .Select(_ => Alphabet[Random.Next(0, Alphabet.Length)]).ToArray());

        public string GetRandomIndexSymbol(string indexShortName = default) => (Random.Next(1) < 1 ? "l" : "s")
                                                                      + Random.Next(1, 20)
                                                                      + (indexShortName ?? GetRandomString(3));

        public string GetRandomCompositionSymbol(string indexShortName = default)
            => GetRandomIndexSymbol(indexShortName) + GetRandomYearMonthSuffix();

        public string GetRandomYearMonthSuffix() => $"{Random.Next(20, 36):00}{Random.Next(1, 13):00}";

        public DateTime GetRandomDateTime()
        {
            var firstJan2020 = new DateTime(2020, 01, 01);
            var firstJan2050 = new DateTime(2050, 01, 01);
            var timeBetween2020And2050 = firstJan2050.Subtract(firstJan2020);

            var randomDay = firstJan2020 + TimeSpan.FromDays(Random.Next(0, (int)timeBetween2020And2050.TotalDays));
            return randomDay;
        }

        public IIndexComposition GetIndexComposition(int componentCount)
        {
            var componentQuantities = Enumerable.Range(0, componentCount)
                .Select(i => GetComponentQuantity(symbol: $"sym{i}", coinGeckoId: $"id-{i}"))
                .ToList();

            var indexComposition = Substitute.For<IIndexComposition>();
            indexComposition.ComponentQuantities.Returns(componentQuantities);
            var randomIndexDefinition = GetRandomIndexDefinition();
            indexComposition.IndexDefinition.Returns(randomIndexDefinition);
            indexComposition.Address.Returns(GetRandomAddressEthereum());
            indexComposition.Symbol.Returns(randomIndexDefinition.Symbol + GetRandomYearMonthSuffix());
            indexComposition.CreationDate.Returns(GetRandomDateTime());
            return indexComposition;
        }

        public IIndexDefinition GetRandomIndexDefinition(string indexSymbol = default, string name = default)
        {
            var indexDefinition = Substitute.For<IIndexDefinition>();
            indexDefinition.NaturalUnit.Returns((ushort)10);
            indexSymbol ??= GetRandomIndexSymbol();
            indexDefinition.Symbol.Returns(indexSymbol);
            name ??= "index name " + GetRandomString(15);
            indexDefinition.Name.Returns(name);
            var description = "description " + GetRandomString(15);
            indexDefinition.Description.Returns(description);
            indexDefinition.Address.Returns(GetRandomAddressEthereum());
            indexDefinition.CreationDate.Returns(GetRandomDateTime());
            return indexDefinition;
        }

        public IComponentQuantity GetComponentQuantity(string address = default,
            string symbol = default,
            string name = default,
            string coinGeckoId = default,
            decimal? quantity = default,
            ushort? decimals = default)
        {
            var componentQuantity = Substitute.For<IComponentQuantity>();
            quantity ??= (decimal)(Random.NextDouble() + 0.001 * 10000);
            componentQuantity.Quantity.Returns(quantity.Value);
            address ??= GetRandomAddressEthereum();
            componentQuantity.ComponentDefinition.Address.Returns(address);
            decimals ??= (ushort)Random.Next(0, 19);
            componentQuantity.ComponentDefinition.Decimals.Returns(decimals.Value);
            symbol ??= GetRandomString(3);
            componentQuantity.ComponentDefinition.Symbol.Returns(symbol);
            name ??= "name " + GetRandomString(24);
            componentQuantity.ComponentDefinition.Name.Returns(name);
            coinGeckoId ??= "id-" + GetRandomString(5);
            componentQuantity.ComponentDefinition.CoinGeckoId.Returns(coinGeckoId);

            return componentQuantity;
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
                    "    \"logIndex\": \"0x46\",\r\n" +
                    "    \"removed\": false,\r\n" +
                    "    \"topics\": [\r\n" +
                    "      \"0xa31e381e140096a837a20ba16eb64e32a4011fda0697adbfd7a8f7341c56aa94\",\r\n" +
                    "      \"0x000000000000000000000000ae81ae0179b38588e05f404e05882a3965d1b415\"\r\n" +
                    "    ],\r\n" +
                    "    \"transactionHash\": \"0x2e39c249e929b8d2dcd2560bd33e1ebd17570742972866b46060bc42bf7c4052\",\r\n" +
                    "    \"transactionIndex\": \"0x80\"\r\n" +
                    "  }" +
                    "\r\n]")
            };
            return transactionReceipt;
        }
    }
}