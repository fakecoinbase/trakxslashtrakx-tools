using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trakx.MarketApi.DataSources.Messari.DTOs;

namespace Trakx.MarketApi.DataSources.Messari.Client
{
    public class MessariApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;
        private readonly ILogger<MessariApiClient> _logger;

        public MessariApiClient(HttpClient httpClient,
            JsonSerializer serializer,
            ILogger<MessariApiClient> logger)
        {
            _httpClient = httpClient;
            _serializer = serializer;
            _logger = logger;
        }
        
        public async Task<AllAssetsResponse> GetAllAssets()
        {
            var path = $"assets";

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ApiEndpoint + path));

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                var streamedContent = await response.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(streamedContent))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var result = _serializer.Deserialize<AllAssetsResponse>(jsonTextReader);
                    return result;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to retrieve all assets", exception);
                return null;
            }
        }
    }
}
