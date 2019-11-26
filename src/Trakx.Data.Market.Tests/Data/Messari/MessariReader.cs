using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Data.Market.Common.Sources.Messari.DTOs;
using Xunit;

namespace Trakx.Data.Market.Tests.Data.Messari
{
    public class MessariReader
    {
        private static readonly Assembly Assembly = typeof(MessariReader).Assembly;
        private static readonly string Namespace = typeof(MessariReader).Namespace ?? string.Empty;

        public async Task<IReadOnlyCollection<Asset>> GetAllAssets()
        {
            var stream = Assembly.GetManifestResourceStream(
                $"{Namespace}.allAssets.json");
            var response = await JsonSerializer.DeserializeAsync<GetAllAssetsResponse>(stream);
            return response.Data.AsReadOnly();
        }

        public async Task<AssetMetrics> GetAssetMetrics(string symbol)
        {
            var response = await GetAssetDetails<GetAssetMetricsResponse>(symbol, "assetMetrics")
                .ConfigureAwait(false);
            return response.Data;
        }

        public async Task<AssetProfile> GetAssetProfile(string symbol)
        {
            var response = await GetAssetDetails<GetAssetProfileResponse>(symbol, "assetProfile")
                .ConfigureAwait(false);
            return response.Data;
        }

        private async Task<T> GetAssetDetails<T>(string symbol, string fileNamePrefix)
        {
            var stream = Assembly.GetManifestResourceStream(
                $"{Namespace}.{fileNamePrefix}.{symbol.ToLower()}.json");
            var response = await JsonSerializer.DeserializeAsync<T>(stream);
            return response;
        }

    }
    public class MessariReaderTests
    {
        [Fact(Skip = "conflicts between windows and linux...")]
        public async Task GetAllAssets_can_deserialise_json()
        {
            var reader = new MessariReader();
            var prices = await reader.GetAllAssets().ConfigureAwait(false);
            prices.Count.Should().Be(20);

            prices.First().Symbol.Should().Be("BTC");
            prices.First().Metrics.MarketData.PriceUsd.Should().Be(7368.463261793677m);
        }

        [Theory]
        [InlineData("BTC", "8081.108675964343")]
        [InlineData("ETH", "173.78629700493275")]
        [InlineData("SYM1", "0.05")]
        [InlineData("SYM2", "0.15")]
        public async Task GetAssetMetrics_can_deserialise_json(string symbol, string expectedPrice)
        {
            var reader = new MessariReader();
            var metrics = await reader.GetAssetMetrics(symbol).ConfigureAwait(false);
            
            metrics.Symbol.Should().Be(symbol);
            metrics.MarketData.PriceUsd.Should().Be(decimal.Parse(expectedPrice));
        }
        
        [Theory]
        [InlineData("BTC", "Native")]
        [InlineData("ETH", "Native")]
        public async Task GetAssetProfile_can_deserialise_json(string symbol, string expectedTokenType)
        {
            var reader = new MessariReader();
            var metrics = await reader.GetAssetProfile(symbol).ConfigureAwait(false);

            metrics.Symbol.Should().Be(symbol);
            metrics.TokenDetails.Type.Should().Be(expectedTokenType);
        }
    }
}
