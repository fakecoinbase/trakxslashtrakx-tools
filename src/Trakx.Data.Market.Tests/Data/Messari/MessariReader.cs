using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Data.Market.Common.Sources.Messari.DTOs;
using Trakx.Data.Market.Tests.Data.Kaiko.AggregatedPrice;
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
                $"{Namespace}.{fileNamePrefix}.{symbol}.json");
            var response = await JsonSerializer.DeserializeAsync<T>(stream);
            return response;
        }

    }
    public class AggregatedPriceReaderTests
    {
        [Fact]
        public async Task AggregatedPriceReader_can_find_existing_symbols()
        {
            var reader = new MessariReader();
            var prices = await reader.GetAllAssets().ConfigureAwait(false);
            prices.Count.Should().Be(2);

            prices.First().Symbol.Should().Be("btc");
            prices.First().Metrics.MarketData.PriceUsd.Should().Be(22.2);
        }
    }
}
