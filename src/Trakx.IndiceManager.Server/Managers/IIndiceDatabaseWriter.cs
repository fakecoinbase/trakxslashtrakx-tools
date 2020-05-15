using System.Threading.Tasks;
using Trakx.Common.Models;

namespace Trakx.IndiceManager.Server.Managers
{
    /// <summary>
    /// This component allows you to interact with the database to record new indices or Compositions.
    /// It also allows to modify indices or compositions stored in the database as long as they are not onChain.
    /// </summary>
    public interface IIndiceDatabaseWriter
    {
        /// <summary>
        /// Tries to save a new composition in the database if it's not published yet.
        /// </summary>
        /// <param name="composition">The composition that we want to save in the database.</param>
        /// <returns>A boolean : true if succeed and false else.</returns>
        Task<bool> TrySaveComposition(IndiceCompositionModel composition);


        /// <summary>
        /// Tries to save a new indice in the database if it's not published yet.
        /// </summary>
        /// <param name="indice">The indice that we want to save in the database.</param>
        /// <returns>A boolean : true if succeed and false else.</returns>
        Task<bool> TrySaveIndice(IndiceDetailModel indice);
    }
}
