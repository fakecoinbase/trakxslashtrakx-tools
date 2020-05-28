using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    public interface IAddressEndpoint
    {
        Task<PagedResponse<AddressResponse>> ListAddressesAsync([CanBeNull] string currency = null,
            [CanBeNull] string state = null, CancellationToken cancellationToken = default);
    }
}