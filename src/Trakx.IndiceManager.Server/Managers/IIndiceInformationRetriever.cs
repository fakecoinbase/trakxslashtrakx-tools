using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.IndiceManager.Server.Managers
{
    public interface IIndiceInformationRetriever
    {
        /// <summary>
        /// Retrieves all of the indices that are in the database.
        /// </summary>
        /// <returns>Details about each indices of the database.</returns>
        Task<List<IIndiceDefinition>> GetAllIndicesFromDatabase();
    }
}
