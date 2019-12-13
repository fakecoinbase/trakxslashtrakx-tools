using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Sources.Messari.DTOs;

namespace Trakx.Data.Market.Common.Sources.Messari.Client
{
    public interface IMessariClient
    {
        Task<IReadOnlyCollection<Asset>> GetAllAssets();
        Task<AssetMetrics> GetMetricsForSymbol(string symbol);
        Task<AssetProfile> GetProfileForSymbol(string symbol);
    }
}