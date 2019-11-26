using System;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Extensions;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;

namespace Trakx.Data.Market.Common.Sources.Kaiko.Client
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
        
        public async Task<AssetsResponse> GetAssets()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.ReferenceDataEndpoint + "assets").ConfigureAwait(false);
                await using var content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<AssetsResponse>(content).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to retrieve assets from reference data", exception);
                return null;
            }
        }

        public async Task<InstrumentsResponse> GetInstruments()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.ReferenceDataEndpoint + "instruments").ConfigureAwait(false);
                await using var content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<InstrumentsResponse>(content);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to retrieve instruments from reference data", exception);
                throw;
            }
        }

        public async Task<ExchangesResponse> GetExchanges()
        {
            try
            {
                var response = await _httpClient.GetAsync(Constants.ReferenceDataEndpoint + "exchanges").ConfigureAwait(false);
                await using var content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<ExchangesResponse>(content).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to retrieve exchanges from reference data", exception);
                throw;
            }
        }

        public async Task<SpotDirectExchangeRateResponse> GetSpotExchangeRate(AggregatedPriceRequest query)
        {
            //todo:  build the URL from a HttpQuery, not manually like that
            var apiPath = query.DirectExchangeRate ? "spot_direct_exchange_rate" : "spot_exchange_rate";

            var startTimeIso8601 = query.StartTime.ToIso8601();
            var endTimeIso8601 = query.StartTime.AddDays(1).ToIso8601();
            var path = $"data/{query.Commodity}.{query.DataVersion}/"
                + $"{apiPath}/{query.BaseAsset}/{query.QuoteAsset}"
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
                await using var content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<SpotDirectExchangeRateResponse>(content).ConfigureAwait(false);
                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to retrieve data for {0}", query.BaseAsset, exception);
                return null;
            }
        }
    }
}
