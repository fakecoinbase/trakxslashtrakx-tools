using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    /// <inheritdoc />
    public class TransactionEndpoint : ITransactionEndpoint
    {
        private readonly ICoinbaseClient _client;

        internal TransactionEndpoint(ICoinbaseClient client)
        {
            _client = client;
        }


        /// <inheritdoc />
        public async Task<PagedResponse<Transaction>> ListTransactionsAsync(string? currency = null, string? state = null, string? walletId = null, string? type = null,
            string? startTime = null, string? endTime = null, string? before = null, string? after = null, int? limit = null, CancellationToken cancellationToken = default)
        {
            return await _client.Request("transactions")
                .SetQueryParams(new
                {
                    before,
                    after,
                    limit,
                    currency,
                    state,
                    type,
                    wallet_id = walletId,
                    start_time = startTime,
                    end_time = endTime
                })
                .GetJsonAsync<PagedResponse<Transaction>>(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Transaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrEmpty(transactionId, nameof(transactionId));
            return await _client.Request("transactions", transactionId)
                .GetJsonAsync<Transaction>(cancellationToken);
        }
    }
}
