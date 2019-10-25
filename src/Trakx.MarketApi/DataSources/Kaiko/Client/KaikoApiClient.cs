using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trakx.MarketApi.DataSources.Kaiko.DTOs;
using Trakx.MarketApi.Extensions;

namespace Trakx.MarketApi.DataSources.Kaiko.Client
{
    public class KaikoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;
        private readonly ILogger<KaikoApiClient> _logger;

        public KaikoApiClient(HttpClient httpClient,
            JsonSerializer serializer,
            ILogger<KaikoApiClient> logger)
        {
            _httpClient = httpClient;
            _serializer = serializer;
            _logger = logger;
        }
        
        public async Task<AssetsResponse> GetAssets()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.ReferenceDataEndpoint + "assets").ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<AssetsResponse>(content);
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to retrieve assets from reference data", exception);
                return null;
            }
        }

        public async Task<InstrumentsResponse> GetInstruments()
        {
            var response = await _httpClient.GetAsync(Constants.ReferenceDataEndpoint + "instruments").ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<InstrumentsResponse>(content);
        }

        public async Task<ExchangesResponse> GetExchanges()
        {
            var response = await _httpClient.GetAsync(Constants.ReferenceDataEndpoint + "exchanges").ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ExchangesResponse>(content);
        }

        public async Task<Response> GetAggregatedPrice(AggregatedPriceRequest query)
        {
            //todo:  build the URL from a HttpQuery, not manually like that
            var startTimeIso8601 = query.StartTime.ToIso8601();
            var endTimeIso8601 = query.StartTime.AddDays(1).ToIso8601();
            var path = $"data/{query.Commodity}.{query.DataVersion}/"
                + $"spot_direct_exchange_rate/{query.BaseAsset}/{query.QuoteAsset}"
                + $"?start_time={UrlEncoder.Default.Encode(startTimeIso8601)}"
                + $"&end_time={UrlEncoder.Default.Encode(endTimeIso8601)}"
                + $"&interval={query.Interval}"
                + $"&page_size={query.PageSize}"
                + (query.Exchanges?.Any() ?? false ? $"&exchanges={string.Join(",", query.Exchanges)}" : "")
                + (query.Sources ? $"&sources={query.Sources.ToString().ToLower()}" : "");

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.MarketDataEndpoint + path));

            try
            {
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                var streamedContent = await response.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(streamedContent))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var result = _serializer.Deserialize<Response>(jsonTextReader);
                    return result;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to retrieve data for {0}", query.BaseAsset, exception);
                return null;
            }
        }
    }
}
