using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Pricing;

namespace Trakx.Common.Interfaces
{
    public interface IIndiceDataProvider
    {
        Task<List<string>> GetAllIndiceSymbols(CancellationToken cancellationToken = default);
        Task<List<string>> GetCompositionSymbolsFromIndice(string indiceSymbol, CancellationToken cancellationToken = default);
        Task<IIndiceComposition?> GetCompositionAtDate(string indiceSymbol, DateTime asOfUtc, CancellationToken cancellationToken = default);
        Task<IIndiceComposition?> GetCompositionFromSymbol(string compositionSymbol, CancellationToken cancellationToken = default);
        Task<IIndiceComposition?> GetCurrentComposition(string indiceSymbol, CancellationToken cancellationToken = default);
        Task<IIndiceValuation> GetInitialValuation(IIndiceComposition composition,
            string quoteCurrency = Constants.DefaultQuoteCurrency,
            CancellationToken cancellationToken = default);
        Task<List<IComponentDefinition>> GetAllComponentsFromCurrentCompositions(CancellationToken cancellationToken = default);
    }
}