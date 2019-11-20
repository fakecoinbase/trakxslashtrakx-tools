using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Sources.Messari.DTOs;

namespace Trakx.Data.Market.Common.Sources.Messari.Client
{
    public class MessariClient : IMessariClient
    {
        private readonly RequestHelperFactory _clientFactory;

        public MessariClient(RequestHelperFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<IReadOnlyCollection<Asset>> GetAllAssets()
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetAllAssets().ConfigureAwait(false);
            return response?.Data ?? new List<Asset>();
        }

        public async Task<AssetMetrics> GetMetricsForSymbol(string symbol)
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetMetricsForSymbol(symbol).ConfigureAwait(false);
            return response?.Data ?? new AssetMetrics();
        }

        public async Task<AssetProfile> GetProfileForSymbol(string symbol)
        {
            var apiClient = _clientFactory.Create();
            var response = await apiClient.GetProfileForSymbol(symbol).ConfigureAwait(false);
            return response?.Data ?? new AssetProfile();
        }
    }
}
