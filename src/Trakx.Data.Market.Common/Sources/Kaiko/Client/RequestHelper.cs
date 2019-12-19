using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
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

        public async Task<SpotDirectExchangeRateResponse> GetSpotExchangeRate(SpotExchangeRateRequest query)
        {
            var apiPath = query.DirectExchangeRate ? "spot_direct_exchange_rate" : "spot_exchange_rate";

            var startTimeIso8601 = query.StartTime.ToIso8601();
            //var endTimeIso8601 = query.StartTime.AddMinutes(1).ToIso8601();

            var path = $"data/{query.Commodity.UrlEncode()}.{query.DataVersion.UrlEncode()}/"
                       + $"{apiPath.UrlEncode()}/{query.BaseAsset.UrlEncode()}/{query.QuoteAsset.UrlEncode()}";

            var queryParams = new Dictionary<string, string>
            {
                {"start_time", startTimeIso8601},
                //{"end_time", endTimeIso8601},
                {"interval", query.Interval},
                {"page_size", query.PageSize.ToString()},
            };
            if (query.Exchanges?.Any() ?? false) queryParams.Add("exchanges", string.Join(",", query.Exchanges));
            if (query.Sources) queryParams.Add("sources", query.Sources.ToString().ToLower());

            var queryString = QueryHelpers.AddQueryString(path, queryParams);

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.MarketDataEndpoint + queryString));

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
