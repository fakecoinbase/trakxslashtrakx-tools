using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Index;
using Trakx.Common.Pricing;

namespace Trakx.Common.Interfaces
{
    public interface IIndexDataProvider
    {
        Task<List<string>> GetAllIndexSymbols(CancellationToken cancellationToken = default);
        Task<List<string>> GetCompositionSymbolsFromIndex(string indexSymbol, CancellationToken cancellationToken = default);
        Task<IIndexComposition?> GetCompositionAtDate(string indexSymbol, DateTime asOfUtc, CancellationToken cancellationToken = default);
        Task<IIndexComposition?> GetCompositionFromSymbol(string compositionSymbol, CancellationToken cancellationToken = default);
        Task<IIndexComposition?> GetCurrentComposition(string indexSymbol, CancellationToken cancellationToken = default);
        Task<IIndexValuation> GetInitialValuation(IIndexComposition composition,
            string quoteCurrency = Constants.DefaultQuoteCurrency,
            CancellationToken cancellationToken = default);
        Task<List<IComponentDefinition>> GetAllComponentsFromCurrentCompositions(CancellationToken cancellationToken = default);
    }
}