using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// This interface allows to modify indices and compositions by interacting with the database.
    /// </summary>
    public interface IIndiceDataModifier
    {
        /// <summary>
        /// This method allows to modify Indice in the database.
        /// </summary>
        /// <param name="indiceDefinitionDao">The indice that we want to modify and save.</param>
        /// <returns>True if the indice was modified successfully, false else.</returns>
        Task<bool> ModifyIndice(IIndiceDefinition indiceDefinitionDao);

        /// <summary>
        /// This method allows to modify Composition in the database.
        /// </summary>
        /// <param name="indiceCompositionDao">The composition that we want to modify and save.</param>
        /// <returns>True if the composition was modified successfully, false else.</returns>
        Task<bool> ModifyComposition(IIndiceComposition indiceCompositionDao);
    }
}
