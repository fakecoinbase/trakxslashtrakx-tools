using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;

namespace Trakx.Data.Market.Common.Sources.Kaiko.Client
{
    public class KaikoClient : IKaikoClient
    {
        private readonly RequestHelperFactory _clientFactory;

        public KaikoClient(RequestHelperFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }
        
        public async Task<SpotDirectExchangeRateResponse> GetSpotExchangeRate(SpotExchangeRateRequest request)
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetSpotExchangeRate(request).ConfigureAwait(false);
            return response;
        }

        [Obsolete("broken")]
        public async Task<AssetsResponse> GetAssets()
        {
            var marketDataClient = _clientFactory.Create();
            var response = await marketDataClient.GetAssets().ConfigureAwait(false);
            return response;
        }

        public async Task<InstrumentsResponse> GetInstruments()
        {
            var marketDataClient = _clientFactory.Create();
            var response = await marketDataClient.GetInstruments().ConfigureAwait(false);
            return response;
        }

        public async Task<ExchangesResponse> GetExchanges()
        {
            var marketDataClient = _clientFactory.Create();
            var response = await marketDataClient.GetExchanges().ConfigureAwait(false);
            return response;
        }

        public SpotExchangeRateRequest CreateSpotExchangeRateRequest(string coinSymbol, string quoteSymbol, bool direct = false,
            DateTime? dateTime = null)
        {
            var queryTime = dateTime?.ToUniversalTime() ?? DateTime.UtcNow;
            var startTime = queryTime.AddHours(-6);
            var query = new SpotExchangeRateRequest
            {
                DataVersion = "latest",
                BaseAsset = coinSymbol.ToLower(),
                Commodity = "trades",
                Exchanges = new List<string>(),
                //Exchanges = Constants.TrustedExchanges,
                Interval = "1m",
                PageSize = 1000,
                QuoteAsset = "usd",
                StartTime = startTime,
                //StartTime = new DateTimeOffset(2019, 11, 26, 00, 00, 00, TimeSpan.Zero),
                //EndTime = new DateTimeOffset(queryTime),
                Sources = false,
                DirectExchangeRate = false
            };
            return query;
        }
    }
}
