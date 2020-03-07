using System;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Pricing;

namespace Trakx.Data.Common.Interfaces
{
    public interface IIndexDataProvider
    {
        Task<IIndexComposition?> GetCompositionAtDate(string indexSymbol, DateTime asOfUtc, CancellationToken cancellationToken = default);
        Task<IIndexComposition?> GetCompositionFromSymbol(string compositionSymbol, CancellationToken cancellationToken = default);
        Task<uint?> GetVersionAtDate(string indexSymbol, DateTime asOfUtc, CancellationToken cancellationToken = default);

        Task<IIndexComposition?> GetCurrentComposition(string indexSymbol, CancellationToken cancellationToken = default);
        Task<uint?> GetCurrentVersion(string indexSymbol, CancellationToken cancellationToken = default);

        Task<IIndexValuation> GetInitialValuation(IIndexComposition composition,
            string quoteCurrency = Constants.DefaultQuoteCurrency,
            CancellationToken cancellationToken = default);

        string GetCacheKeyForCurrentComposition(string indexSymbol);
        Task<string?> CacheCurrentComposition(string indexSymbol, CancellationToken cancellationToken);
    }
}