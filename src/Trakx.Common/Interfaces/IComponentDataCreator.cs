using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Use this class to save new componentDefinitions in the database
    /// </summary>
    public interface IComponentDataCreator
    {
        /// <summary>
        /// Tries to put new <see cref="IComponentDefinition"/> in the database.
        /// </summary>
        /// <param name="componentDefinition">The component that we want to save.</param>
        /// <returns>A boolean : true if addition succeed, false else.</returns>
        Task<bool> TryAddComponentDefinition(IComponentDefinition componentDefinition);
    }
}