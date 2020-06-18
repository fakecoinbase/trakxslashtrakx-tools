using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    /// <inheritdoc />
    public class AddressEndpoint : IAddressEndpoint
    {
        private readonly ICoinbaseClient _client;

        internal AddressEndpoint(ICoinbaseClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<AddressResponse>> ListAddressesAsync(string? currency = null,
             string? state = null, string? before = null, string? after = null, int? limit = null, CancellationToken cancellationToken = default)
        {
            return await _client
                .Request("addresses")
                .SetQueryParams(new { currency, state,before,after,limit })
                .GetJsonAsync<PagedResponse<AddressResponse>>(cancellationToken);
        }
    }
}
