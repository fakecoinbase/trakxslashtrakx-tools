using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using JetBrains.Annotations;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client
{
    public class AddressEndpoint : IAddressEndpoint
    {
        private readonly ICoinbaseClient _client;

        public AddressEndpoint(ICoinbaseClient client)
        {
            _client = client;
        }

        public async Task<PagedResponse<AddressResponse>> ListAddressesAsync([CanBeNull] string currency = null,
            [CanBeNull] string state = null, CancellationToken cancellationToken = default)
        {
            return await _client
                .Request("addresses")
                .SetQueryParams(new { currency, state })
                .GetJsonAsync<PagedResponse<AddressResponse>>(cancellationToken);
        }
    }
}
