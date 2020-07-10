using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flurl.Http;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Endpoints
{
    /// <inheritdoc />
    internal class TransactionEndpoint : ITransactionEndpoint
    {
        private readonly IFlurlClient _client;

        public TransactionEndpoint(IFlurlClient client)
        {
            _client = client;
        }


        /// <inheritdoc />
        public async Task<PagedResponse<CoinbaseRawTransaction>> ListTransactionsAsync(string? currency = null, 
            TransactionState? state = null, 
            string? walletId = null, 
            TransactionType? type = null,
            DateTime? startTime = null, 
            DateTime? endTime = null, 
            PaginationOptions? paginationOptions = default,
            CancellationToken cancellationToken = default)
        {
            paginationOptions ??= PaginationOptions.Default;
            var page= await _client.Request("transactions")
                .SetQueryParams(new
                {
                    currency,
                    state=state?.ToString(),
                    type=type?.ToString(),
                    wallet_id = walletId,
                    start_time = startTime?.ToString("o"),
                    end_time = endTime?.ToString("o"),
                    before = paginationOptions?.Before,
                    after = paginationOptions?.After,
                    limit = paginationOptions?.PageSize,
                })
                .GetJsonAsync<PagedResponse<CoinbaseRawTransaction>>(cancellationToken);
            return page;
        }


        /// <inheritdoc />
        public async Task<CoinbaseRawTransaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrEmpty(transactionId, nameof(transactionId));
            return await _client.Request("transactions", transactionId)
                .GetJsonAsync<CoinbaseRawTransaction>(cancellationToken);
        }
    }
}
