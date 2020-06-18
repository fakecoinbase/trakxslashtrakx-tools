using System.Threading;
using System.Threading.Tasks;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    /// <summary>
    /// Use this Endpoint to retrieve data about Addresses on Coinbase Custody
    /// </summary>
    public interface IAddressEndpoint
    {
        /// <summary>
        /// Allows to retrieve all addresses available on Coinbase Custody API
        /// </summary>
        /// <param name="currency">The currency of the address, optional parameter</param>
        /// <param name="state">The <see cref="AddressState"/>, optional parameter</param>
        /// <param name="before">Request page before (newer than) this pagination id.</param>
        /// <param name="after">Request page after (older than) this pagination id.</param>
        /// <param name="limit">Number of results per request. Maximum 100. Default 25.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="PagedResponse{AddressResponse}"/> if request succeed.</returns>
        Task<PagedResponse<AddressResponse>> ListAddressesAsync(string? currency = null,
            string? state = null, string? before = null, string? after = null, int? limit = null,
            CancellationToken cancellationToken = default);
    }
}