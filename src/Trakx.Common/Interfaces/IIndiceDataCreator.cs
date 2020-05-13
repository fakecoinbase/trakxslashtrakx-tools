using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// This interface allows to add indices and compositions by interacting with the database.
    /// </summary>
    public interface IIndiceDataCreator
    {
        /// <summary>
        /// This method allows to add Composition in the database.
        /// </summary>
        /// <param name="indiceCompositionDao">The composition that we want to add and save.</param>
        /// <param name="indiceDefinitionDao">The indice Definition associated to the composition</param>
        /// <returns>True if the composition was added successfully, false else.</returns>
        Task<bool> AddNewComposition(IIndiceComposition indiceCompositionDao, IIndiceDefinition? indiceDefinitionDao);


        /// <summary>
        /// This method allows to add Indice in the database.
        /// </summary>
        /// <param name="indiceDefinitionDao">The indice that we want to add and save.</param>
        /// <returns>True if the indice was added successfully, false else.</returns>
        Task<bool> AddNewIndice(IIndiceDefinition indiceDefinitionDao);
    }
}
