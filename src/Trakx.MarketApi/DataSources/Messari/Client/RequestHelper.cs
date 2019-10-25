using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.MarketApi.DataSources.Messari.DTOs;

namespace Trakx.MarketApi.DataSources.Messari.Client
{
    public class RequestHelper
    {
        private readonly MessariApiClientFactory _clientFactory;

        public RequestHelper(MessariApiClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<IReadOnlyCollection<Asset>> GetAggregatedPrices()
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetAllAssets().ConfigureAwait(false);
            if (response?.Data == null) return new List<Asset>();
            return response.Data;
        }
    }
}
