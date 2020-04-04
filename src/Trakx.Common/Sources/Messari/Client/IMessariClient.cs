using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Data.Common.Sources.Messari.DTOs;

namespace Trakx.Data.Common.Sources.Messari.Client
{
    public interface IMessariClient
    {
        IAsyncEnumerable<Asset> GetAllAssetsAsync(CancellationToken cancellationToken = default);
        Task<List<Asset>> GetAllAssets(CancellationToken cancellationToken = default);
        Task<AssetMetrics> GetMetricsForSymbol(string symbol);
        Task<AssetProfile> GetProfileForSymbol(string symbol);
        List<string> SelectedSectors { get; }

        Task<decimal?> GetLatestPrice(string symbol,
            string quoteCurrency = Pricing.Constants.DefaultQuoteCurrency);

        Task<Dictionary<string, decimal?>> GetLatestPrices(IEnumerable<string> symbols,
            string quoteCurrency = Pricing.Constants.DefaultQuoteCurrency);
    }
}