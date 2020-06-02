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
    public class TransactionEndpoint : ITransactionEndpoint
    {
        private readonly ICoinbaseClient _client;

        public TransactionEndpoint(ICoinbaseClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PagedResponse<Transaction>> ListTransactionsAsync([CanBeNull] string currency=null, [CanBeNull] string startTime=null, [CanBeNull] string endTime=null, [CanBeNull] string state=null, [CanBeNull] string walletId=null, [CanBeNull] string type=null, CancellationToken cancellationToken=default)
        {
            return await _client.Request("transactions")
                .SetQueryParams(new
                { currency,state,type, 
                    wallet_id =walletId, 
                    start_time = startTime, 
                    end_time = endTime
                })
                .GetJsonAsync<PagedResponse<Transaction>>(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Transaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken=default)
        {
            Guard.Against.NullOrEmpty(transactionId, nameof(transactionId));
            return await _client.Request("transactions", transactionId)
                .GetJsonAsync<Transaction>(cancellationToken);
        }
    }
}
