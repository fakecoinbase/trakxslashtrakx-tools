using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Trakx.Common.Sources.Messari.DTOs;

namespace Trakx.Common.Sources.Messari.Client
{
    public class RequestHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RequestHelper> _logger;

        public RequestHelper(HttpClient httpClient,
            ILogger<RequestHelper> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async IAsyncEnumerable<Asset> GetAllAssets(
            [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            const string path = "assets";
            var i = 0;
            while(!cancellationToken.IsCancellationRequested && i < 1_000_000)
            {
                i++;
                var queryParams = new Dictionary<string, string>
                {
                    {"limit", "500"},
                    {"page", i.ToString()},
                };
                var queryString = QueryHelpers.AddQueryString(path, queryParams);
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ApiEndpoint + queryString));

                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                await using var streamedContent = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer
                    .DeserializeAsync<GetAllAssetsResponse>(streamedContent, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                if(result?.Data == null || result.Data.Count == 0) yield break;

                using (var enumerator = result.Data.GetEnumerator())
                    while (enumerator.MoveNext()) yield return enumerator.Current;
            }


        }

        public async Task<GetAssetProfileResponse?> GetProfileForSymbol(string symbol)
        {
            var path = $"assets/{symbol}/profile";

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ApiEndpoint + path));

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                await using var streamed = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<GetAssetProfileResponse>(streamed).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to profile for {symbol}", symbol);
                return default;
            }
        }

        public async Task<GetAssetMetricsResponse?> GetMetricsForSymbol(string symbol)
        {
            var path = $"assets/{symbol}/metrics";

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ApiEndpoint + path));

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                await using var streamed = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<GetAssetMetricsResponse>(streamed).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to metrics data for {symbol}", symbol);
                return null;
            }
        }

        public async Task<GetAssetMetricsResponse?> GetMarketDataForSymbol(string symbol)
        {
            var path = $"assets/{symbol}/metrics/market-data";
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ApiEndpoint + path));

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                await using var streamed = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<GetAssetMetricsResponse>(streamed).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to market data for {symbol}", symbol);
                return default;
            }
        }
    }
}
