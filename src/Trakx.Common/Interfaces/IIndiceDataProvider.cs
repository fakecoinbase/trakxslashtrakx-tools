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
        /// <summary>
        /// Retrieves all the CTIs defined in the database, and returns the corresponding symbols.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A list of all the CTI symbols ever defined by Trakx.</returns>
        Task<List<string>> GetAllIndiceSymbols(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all the of the compositions associated with a give CTI. Each rebalancing corresponds to a new composition.
        /// </summary>
        /// <param name="indiceSymbol">The symbol of the CTI for which we want to get the compositions.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>The list of all compositions associated to a given CTI.</returns>
        Task<List<string>> GetCompositionSymbolsFromIndice(string indiceSymbol, CancellationToken cancellationToken = default);

        Task<IIndiceComposition?> GetCompositionAtDate(string indiceSymbol, DateTime asOfUtc, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a composition from database using its unique symbol.
        /// </summary>
        /// <param name="compositionSymbol">The unique symbol used to identify the composition.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>The details about the composition with symbol <param name="compositionSymbol"></param></returns>
        Task<IIndiceComposition?> GetCompositionFromSymbol(string compositionSymbol, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current composition of a CTI, based on its symbol.
        /// </summary>
        /// <param name="indiceSymbol">The unique symbol of the CTI.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>The currently traded composition of the CTI.</returns>
        Task<IIndiceComposition?> GetCurrentComposition(string indiceSymbol, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the initial valuation for a given CTI valuation. This valuation is the valuation as of
        /// <see cref="IIndiceComposition.CreationDate"/> of the composition.
        /// </summary>
        /// <param name="composition">The CTI composition for which we need valuation.</param>
        /// <param name="quoteCurrency">The currency in which we want the valuation to be expressed.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>The valuation of the CTI at the time of its creation.</returns>
        Task<IIndiceValuation> GetInitialValuation(IIndiceComposition composition,
            string quoteCurrency = Constants.DefaultQuoteCurrency,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Use this method to get all the Components currently being used in the indices available to trade on the
        /// Trakx exchange. This can be useful for subscribing to the corresponding price feeds for instance.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A list of the active component definitions.</returns>
        Task<List<IComponentDefinition>> GetAllComponentsFromCurrentCompositions(CancellationToken cancellationToken = default);


        /// <summary>
        /// Allows to retrieve all the compositions associated with an indice.
        /// </summary>
        /// <param name="indiceSymbol">The symbol of the indice for which we want all of the compositions.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A list of <see cref="IIndiceComposition"/> associated to the indice.</returns>
        Task<List<IIndiceComposition>?> GetAllCompositionForIndice(string indiceSymbol, CancellationToken cancellationToken = default);


        /// <summary>
        /// Retrieves all of the indices that are in the database.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>Details about each indices of the database.</returns>
        Task<List<IIndiceDefinition>> GetAllIndices(CancellationToken cancellationToken = default);


        /// <summary>
        /// Verify if the address of the indice exists in the database.
        /// </summary>
        /// <param name="indiceAddress">The Ethereum address of the indice we looking for.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>Return a boolean : true if the indice exists, false else.</returns>
        Task<bool> TryToGetIndiceByAddress(string indiceAddress, CancellationToken cancellationToken = default);


        /// <summary>
        /// Verify if the address of the composition exists in the database
        /// </summary>
        /// <param name="compositionAddress">The Ethereum address of the composition we looking for.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>Return a boolean : true if the composition exists, false else.</returns>
        Task<bool> TryToGetCompositionByAddress(string compositionAddress, CancellationToken cancellationToken = default);
    }
}