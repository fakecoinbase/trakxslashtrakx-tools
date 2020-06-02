using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flurl.Http;
using JetBrains.Annotations;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    /// <inheritdoc />
    public class WalletEndpoint : IWalletEndpoint
    {
        private readonly ICoinbaseClient _client;

        public WalletEndpoint(ICoinbaseClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Wallet>> ListWalletsAsync([CanBeNull] string currency=null, CancellationToken cancellationToken=default)
        {
            return await _client.Request("wallets")
                .SetQueryParam("currency", currency)
                .GetJsonAsync<PagedResponse<Wallet>>(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Wallet> GetWalletAsync(string walletId, CancellationToken cancellationToken=default)
        {
            Guard.Against.NullOrEmpty(walletId,nameof(walletId));
            return await _client.Request("wallets", walletId)
                .GetJsonAsync<Wallet>(cancellationToken);
        }

    }
}
