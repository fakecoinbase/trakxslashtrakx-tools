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
        /// <param name="paginationOptions">Optional custom pagination options. Allows to start searching from a custom point
        /// in time, or to change default number of results returned per page.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="PagedResponse{AddressResponse}"/> if request succeed.</returns>
        Task<PagedResponse<AddressResponse>> ListAddressesAsync(string? currency = null,
            string? state = null, PaginationOptions? paginationOptions = default,
            CancellationToken cancellationToken = default);
    }
}