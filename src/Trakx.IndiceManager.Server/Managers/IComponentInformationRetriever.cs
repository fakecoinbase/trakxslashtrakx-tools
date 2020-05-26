
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Models;

namespace Trakx.IndiceManager.Server.Managers
{
    /// <summary>
    /// Use this component to retrieve information about different ERC20 tokens
    /// used as components in indices.
    /// </summary>
    public interface IComponentInformationRetriever
    {
        /// <summary>
        /// Retrieves a definition for a component given its address.
        /// </summary>
        /// <param name="address">The Ethereum address of the component.</param>
        /// <returns>Definition for the component as retrieved from database, or from
        /// information retrieved online (web3, CryptoCompare).</returns>
        Task<IComponentDefinition> GetComponentDefinitionFromAddress(string address);

        /// <summary>
        /// Retrieves all of the components that are currently stored in the database.
        /// </summary>
        /// <returns>A list of <see cref="IComponentDefinition"/> and an empty list if there is no components in the database.</returns>
        Task<List<IComponentDefinition>> GetAllComponents();

        /// <summary>
        /// Tries to save a new component definition in the database.
        /// </summary>
        /// <param name="componentDetailModel">The component that we want to save.</param>
        /// <returns>An object with a response 201 if the adding was successful</returns>
        Task<bool> TryToSaveComponentDefinition(ComponentDetailModel componentDetailModel);
    }
}