using System;
using System.Collections.Generic;
using System.Threading;
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

        public IAsyncEnumerable<Asset> GetAllAssetsAsync(CancellationToken cancellationToken = default)
        {
            var apiClient = _clientFactory.Create();
            var response = apiClient.GetAllAssets();
            return response;
        }

        public async Task<List<Asset>> GetAllAssets(CancellationToken cancellationToken = default)
        {
            var result = new List<Asset>();
            var apiClient = _clientFactory.Create();
            var assets = apiClient.GetAllAssets(cancellationToken);
            await foreach (var asset in assets.ConfigureAwait(false))
            {
                result.Add(asset);
            }

            return result;
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
