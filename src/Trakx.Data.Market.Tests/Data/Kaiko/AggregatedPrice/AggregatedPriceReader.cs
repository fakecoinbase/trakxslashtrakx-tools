using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using AggregatedPriceDto = Trakx.Data.Market.Common.Sources.Kaiko.DTOs.AggregatedPrice;

namespace Trakx.Data.Market.Tests.Data.Kaiko.AggregatedPrice
{
    public class AggregatedPriceReader
    {
        private static readonly Assembly Assembly = typeof(AggregatedPriceReader).Assembly;
        private static readonly string Namespace = typeof(AggregatedPriceReader).Namespace ?? string.Empty;

        public async Task<IReadOnlyCollection<AggregatedPriceDto>> GetAggregatePriceForSymbol(string symbol)
        {
            var stream = Assembly.GetManifestResourceStream(
                $"{Namespace}.{symbol.ToLower()}.json");
            var aggregatePriceForSymbol = await JsonSerializer.DeserializeAsync<AggregatedPriceDto[]>(stream);
            return aggregatePriceForSymbol.ToList().AsReadOnly();
        }
    }

    public class AggregatedPriceReaderTests
    {
        [Fact]
        public async Task AggregatedPriceReader_can_find_existing_symbols()
        {
            var reader = new AggregatedPriceReader();
            var prices = await reader.GetAggregatePriceForSymbol("SYM1").ConfigureAwait(false);
            prices.Count.Should().Be(2);

            prices.First().Price.Should().Be("0.04");
        }

        [Fact]
        public void AggregatedPriceReader_throws_on_unknown_symbol()
        {
            var reader = new AggregatedPriceReader();
            new Action( () =>
                    reader.GetAggregatePriceForSymbol("NOPE").Wait())
                .Should().Throw<Exception>();
        }
    }
}