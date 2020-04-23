
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;

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
    }
}