using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;
using Trakx.IndiceManager.Server.Models;

namespace Trakx.IndiceManager.Server.Managers
{
    public interface IIndiceInformationRetriever
    {
        /// <summary>
        /// Retrieves all of the indices that are in the database.
        /// </summary>
        /// <returns>Details about each indices of the database.</returns>
        Task<List<IIndiceDefinition>> GetAllIndicesFromDatabase();

        /// <summary>
        /// Retrieves all of the compositions for a given indice.
        /// </summary>
        /// <param name="symbol">The symbol of the indice for which we want the compositions</param>
        /// <returns>All compositions for a indice, both old and new.</returns>
        Task<List<IIndiceComposition>> GetAllCompositionsFromDatabase(string symbol);

        /// <summary>
        /// Tries to save a new indice in the database.
        /// </summary>
        /// <param name="indice">The indice that we want to save in the database.</param>
        /// <returns>A boolean : true if succeed and false else.</returns>
        Task<bool> TrySaveIndice(IndiceDetailModel indice);

        /// <summary>
        /// Tries to save a new composition in the database.
        /// </summary>
        /// <param name="composition">The composition that we want to save in the database.</param>
        /// <returns>A boolean : true if succeed and false else.</returns>
        Task<bool> TrySaveComposition(IndiceCompositionModel composition);

        /// <summary>
        /// Verify if the address of the indice exists in the database.
        /// </summary>
        /// <param name="indiceAddress">The Ethereum address of the indice we looking for.</param>
        /// <returns>Return a boolean : true if the indice exists, false else.</returns>
        Task<bool> SearchIndice(string indiceAddress);


        /// <summary>
        /// Verify if the address of the composition exists in the database
        /// </summary>
        /// <param name="compositionAddress">The Ethereum address of the composition we looking for.</param>
        /// <returns>Return a boolean : true if the composition exists, false else.</returns>
        Task<bool> SearchComposition(string compositionAddress);
    }
}
