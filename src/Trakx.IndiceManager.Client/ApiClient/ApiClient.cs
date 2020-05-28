using System.Threading;
using System.Threading.Tasks;
using Trakx.Common.Models;

namespace Trakx.IndiceManager.Client.ApiClient
{
    /// <summary>
    /// Interface through which the client calls the REST methods on the server.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Send a HTTP request to the corresponding method on the IndexController.
        /// </summary>
        /// <param name="composition">Model containing all the details of the indice to save</param>
        /// <param name="cancellationToken">Use this token to cancel the asynchronous task.</param>
        Task<bool> TrySaveIndiceCompositionAsync(IndiceCompositionModel composition, CancellationToken cancellationToken);
    }
}
