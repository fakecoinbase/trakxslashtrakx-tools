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


        /// <summary>
        /// Allows to retrieve all the compositions associated with an indice.
        /// </summary>
        /// <param name="indiceSymbol">The symbol of the indice for which we want all of the compositions.</param>
        /// <returns>A list of <see cref="IIndiceComposition"/> associated to the indice.</returns>
        Task<List<IIndiceComposition>?> GetAllCompositionForIndice(string indiceSymbol);


        /// <summary>
        /// Retrieves all of the indices that are in the database.
        /// </summary>
        /// <returns>Details about each indices of the database.</returns>
        Task<List<IIndiceDefinition>> GetAllIndices();


        /// <summary>
        /// Verify if the address of the indice exists in the database.
        /// </summary>
        /// <param name="indiceAddress">The Ethereum address of the indice we looking for.</param>
        /// <returns>Return a boolean : true if the indice exists, false else.</returns>
        Task<bool> TryToGetIndiceByAddress(string indiceAddress);


        /// <summary>
        /// Verify if the address of the composition exists in the database
        /// </summary>
        /// <param name="compositionAddress">The Ethereum address of the composition we looking for.</param>
        /// <returns>Return a boolean : true if the composition exists, false else.</returns>
        Task<bool> TryToGetCompositionByAddress(string compositionAddress);
    }
}