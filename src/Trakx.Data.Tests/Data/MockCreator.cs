using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Tests.Data
{
    public class MockCreator
    {
        private static readonly Random Random = new Random();
        private const string AddressChars = "abcdef01234566789";
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        public string GetRandomAddressEthereum() => "0x" + new string(Enumerable.Repeat(
            Random.Next(0, AddressChars.Length), 40).Select(i => AddressChars[i]).ToArray());

        public string GetRandomString(int size) => new string(Enumerable.Repeat(
            Random.Next(0, Alphabet.Length), size).Select(i => Alphabet[i]).ToArray());

        public string GetRandomIndexSymbol(string indexShortName = default) => (Random.Next(1) < 1 ? "l" : "s")
                                                                      + Random.Next(1, 20)
                                                                      + indexShortName ?? GetRandomString(3);

        public string GetRandomCompositionSymbol(string indexShortName = default)
            => GetRandomIndexSymbol(indexShortName) + GetRandomYearMonthSuffix();

        public string GetRandomYearMonthSuffix()
        {
            return $"{Random.Next(20, 36)}{Random.Next(1, 13)}";
        }

        public IIndexComposition GetIndexComposition()
        {
            var indexDefinition = GetRandomIndexDefinition();

            var componentQuantities = new List<IComponentQuantity>
            {
                GetComponentQuantity(),
                GetComponentQuantity(),
                GetComponentQuantity(),
            };

            var indexComposition = Substitute.For<IIndexComposition>();
            indexComposition.ComponentQuantities.Returns(componentQuantities);
            var randomIndexDefinition = GetRandomIndexDefinition();
            indexComposition.IndexDefinition.Returns(randomIndexDefinition);
            indexComposition.Address.Returns(GetRandomAddressEthereum());
            indexComposition.Symbol.Returns(randomIndexDefinition.Symbol + GetRandomYearMonthSuffix());
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
            name ??= "description " + GetRandomString(15);
            indexDefinition.Description.Returns(name);
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
    }
}