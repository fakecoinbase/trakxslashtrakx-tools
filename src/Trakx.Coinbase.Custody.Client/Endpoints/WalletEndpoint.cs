using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    /// <inheritdoc />
    public class WalletEndpoint : IWalletEndpoint
    {
        private readonly ICoinbaseClient _client;

        internal WalletEndpoint(ICoinbaseClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Wallet>> ListWalletsAsync(string? currency = null, string? before = null, string? after = null, int? limit = null, CancellationToken cancellationToken = default)
        {
            return await _client.Request("wallets")
                .SetQueryParams(new{currency,after,before,limit})
                .GetJsonAsync<PagedResponse<Wallet>>(cancellationToken);
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
