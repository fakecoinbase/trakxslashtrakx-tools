﻿using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    /// <inheritdoc />
    internal class AddressEndpoint : IAddressEndpoint
    {
        private readonly IFlurlClient _client;

        public AddressEndpoint(IFlurlClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<AddressResponse>> ListAddressesAsync(string? currency = null,
            string? state = null, PaginationOptions? paginationOptions = default, CancellationToken cancellationToken = default)
        {
            paginationOptions ??= PaginationOptions.Default;
            var page = await _client
                .Request("addresses")
                .SetQueryParams(new
                {
                    currency,
                    state,
                    before = paginationOptions.Before,
                    after = paginationOptions.After,
                    limit = paginationOptions.PageSize,
                })
                .GetJsonAsync<PagedResponse<AddressResponse>>(cancellationToken);

            return page;
        }
    }
}
