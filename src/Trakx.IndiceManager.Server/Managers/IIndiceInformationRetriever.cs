using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.IndiceManager.Server.Managers
{
    /// <summary>
    /// This component allows you to retrieve indices and compositions from the database.
    /// </summary>
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
        Task<List<IIndiceComposition>?> GetAllCompositionForIndiceFromDatabase(string symbol);

        
        /// <summary>
        /// Verify if the address of the indice exists in the database.
        /// </summary>
        /// <param name="indiceAddress">The Ethereum address of the indice we looking for.</param>
        /// <returns>Return a boolean : true if the indice exists, false else.</returns>
        Task<bool> SearchIndiceByAddress(string? indiceAddress);


        /// <summary>
        /// Verify if the address of the composition exists in the database
        /// </summary>
        /// <param name="compositionAddress">The Ethereum address of the composition we looking for.</param>
        /// <returns>Return a boolean : true if the composition exists, false else.</returns>
        Task<bool> SearchCompositionByAddress(string compositionAddress);
    }
}
