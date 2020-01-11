using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Sources.Messari.DTOs;

namespace Trakx.Data.Market.Common.Sources.Messari.Client
{
    public interface IMessariClient
    {
        IAsyncEnumerable<Asset> GetAllAssetsAsync(CancellationToken cancellationToken = default);
        Task<List<Asset>> GetAllAssets(CancellationToken cancellationToken = default);
        Task<AssetMetrics> GetMetricsForSymbol(string symbol);
        Task<AssetProfile> GetProfileForSymbol(string symbol);
        List<string> SelectedSectors { get; }
    }
}