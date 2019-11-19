using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Sources.Messari.DTOs;

namespace Trakx.Data.Market.Common.Sources.Messari.Client
{
    public class MessariApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MessariApiClient> _logger;

        public MessariApiClient(HttpClient httpClient,
            ILogger<MessariApiClient> logger)
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

        public async Task<GetAssetProfileResponse> GetProfileBySymbol(string symbol)
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

        public async Task<GetAllAssetsResponse> GetMetricsBySymbol(string symbol)
        {
            var path = $"assets/{symbol}/metrics";

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ApiEndpoint + path));

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                await using var streamed = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<GetAllAssetsResponse>(streamed).ConfigureAwait(false);
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
