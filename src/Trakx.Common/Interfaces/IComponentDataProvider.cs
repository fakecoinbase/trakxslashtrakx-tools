using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Use this component to retrieve data about components stored in database.
    /// </summary>
    public interface IComponentDataProvider
    {
        /// <summary>
        /// Allows to retrieves all of the components that are currently stored in the database.
        /// </summary>
        /// <returns>A list of <see cref="IComponentDefinition"/> or an empty list if there is no components in the database.</returns>
        Task<List<IComponentDefinition>> GetAllComponentsFromDatabase(CancellationToken cancellationToken = default);

        /// <summary>
        /// Allows to retrieve a component by its address only if it is in the database.
        /// </summary>
        /// <param name="componentAddress">The Ethereum address of the component that we're looking for.</param>
        /// <returns>A <see cref="IComponentDefinition"/>.</returns>
        Task<IComponentDefinition> GetComponentFromDatabaseByAddress(string componentAddress, CancellationToken cancellationToken = default);
    }
}