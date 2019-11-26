using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Sources.Messari.DTOs;

namespace Trakx.Data.Market.Common.Sources.Messari.Client
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
        
        public async Task<GetAllAssetsResponse> GetAllAssets()
        {
            var path = $"assets";

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ApiEndpoint + path));

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                await using var streamedContent = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<GetAllAssetsResponse>(streamedContent).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to retrieve all assets", exception);
                return null;
            }
        }

        public async Task<GetAssetProfileResponse> GetProfileForSymbol(string symbol)
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
                _logger.LogError("Failed to retrieve all assets", exception);
                return null;
            }
        }

        public async Task<GetAssetMetricsResponse> GetMetricsForSymbol(string symbol)
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
                _logger.LogError("Failed to retrieve all assets", exception);
                return null;
            }
        }
    }
}
