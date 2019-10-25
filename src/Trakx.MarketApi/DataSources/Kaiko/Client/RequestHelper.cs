using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.MarketApi.DataSources.Kaiko.DTOs;

namespace Trakx.MarketApi.DataSources.Kaiko.Client
{
    public class RequestHelper
    {
        private readonly KaikoApiClientFactory _clientFactory;

        public RequestHelper(KaikoApiClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<IReadOnlyCollection<AggregatedPrice>> GetAggregatedPrices(AggregatedPriceRequest request)
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetAggregatedPrice(request).ConfigureAwait(false);
            if (response?.Result != Constants.SuccessResponse || response?.Data == null) return new AggregatedPrice[0];
            return response.Data;
        }

        public async Task<IReadOnlyCollection<Asset>> GetAssets()
        {
            var marketDataClient = _clientFactory.Create();
            var response = await marketDataClient.GetAssets().ConfigureAwait(false);
            if (response?.Result != Constants.SuccessResponse || response?.Data == null) return new Asset[0];
            return response.Data;
        }

        public async Task<IReadOnlyCollection<Instrument>> GetInstruments()
        {
            var marketDataClient = _clientFactory.Create();
            var response = await marketDataClient.GetInstruments().ConfigureAwait(false);
            if (response?.Result != Constants.SuccessResponse || response?.Data == null) return new Instrument[0];
            return response.Data;
        }

        public async Task<IReadOnlyCollection<Exchange>> GetExchanges()
        {
            var marketDataClient = _clientFactory.Create();
            var response = await marketDataClient.GetExchanges().ConfigureAwait(false);
            if (response?.Result != Constants.SuccessResponse || response?.Data == null) return new Exchange[0];
            return response.Data;
        }
    }
}
