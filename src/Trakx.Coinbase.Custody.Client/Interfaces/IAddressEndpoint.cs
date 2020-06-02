using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        /// <returns>A <see cref="PagedResponse{AddressResponse}"/> if request succeed.</returns>
        Task<PagedResponse<AddressResponse>> ListAddressesAsync([CanBeNull] string currency = null,
            [CanBeNull] string state = null, CancellationToken cancellationToken = default);
    }
}