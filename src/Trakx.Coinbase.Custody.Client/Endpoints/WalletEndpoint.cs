using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    /// <inheritdoc />
    internal class WalletEndpoint : IWalletEndpoint
    {
        private readonly IFlurlClient _client;

        public WalletEndpoint(IFlurlClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Wallet>> ListWalletsAsync(string? currency = null, 
            PaginationOptions? paginationOptions = default, CancellationToken cancellationToken = default)
        {
            paginationOptions ??= PaginationOptions.Default;
            var page = await _client.Request("wallets")
                .SetQueryParams(new
                {
                    currency,
                    before = paginationOptions.Before,
                    after = paginationOptions.After,
                    limit = paginationOptions.PageSize,
                })
                .GetJsonAsync<PagedResponse<Wallet>>(cancellationToken);
            return page;
        }

        /// <inheritdoc />
        public async Task<Wallet> GetWalletAsync(string walletId, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrEmpty(walletId, nameof(walletId));
            return await _client.Request("wallets", walletId)
                .GetJsonAsync<Wallet>(cancellationToken);
        }

    }
}
