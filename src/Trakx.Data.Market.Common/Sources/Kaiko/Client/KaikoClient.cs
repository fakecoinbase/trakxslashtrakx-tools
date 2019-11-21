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

        public async Task<SpotDirectExchangeRateResponse> GetSpotExchangeRate(AggregatedPriceRequest request)
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetSpotExchangeRate(request).ConfigureAwait(false);
            return response;
        }

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
    }
}
