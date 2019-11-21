using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute.Extensions;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;
using Xunit;

namespace Trakx.Data.Market.Tests.Data.Kaiko.AggregatedPrice
{
    public class KaikoReader
    {
        private static readonly Assembly Assembly = typeof(KaikoReader).Assembly;
        private static readonly string Namespace = typeof(KaikoReader).Namespace ?? string.Empty;

        public async Task<SpotDirectExchangeRateResponse> GetSpotExchangeRateForSymbol(string symbol, bool direct)
        {
            var directPrefix = direct ? "direct" : "detailed";
            var stream = Assembly.GetManifestResourceStream(
                $"{Namespace}.{directPrefix}.{symbol.ToLower()}.json");
            var response = await JsonSerializer.DeserializeAsync<SpotDirectExchangeRateResponse>(stream);
            return response;
        }

    }

    public class KaikoReaderTests
    {
        [Fact]
        public async Task GetSpotExchangeRateForSymbol_can_find_existing_symbols()
        {
            var reader = new KaikoReader();
            var response = await reader.GetSpotExchangeRateForSymbol("SYM1", false).ConfigureAwait(false);
            response.Data.Count.Should().Be(3);

            response.Data.First().Price.Should().Be("0.045");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetSpotExchangeRateForSymbol_throws_on_unknown_symbol(bool direct)
        {
            var reader = new KaikoReader();
            new Action( () =>
                    reader.GetSpotExchangeRateForSymbol("NOPE", direct).Wait())
                .Should().Throw<Exception>();
        }

        [Fact]
        public async Task GetSpotDirectExchangeRate_can_find_existing_symbols()
        {
            var reader = new KaikoReader();
            var response = await reader.GetSpotExchangeRateForSymbol("BTG", true).ConfigureAwait(false);
            response.Data.Count.Should().Be(1);

            response.Data.First().Price.Should().Be("7.6735451346133718492896844116483257495112942051604");
        }
    }
}