using System.Threading;
using System.Threading.Tasks;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.Coinbase.Custody.Client.Interfaces
{
    /// <summary>
    /// Use this Endpoint to retrieve data about transactions
    /// </summary>
    public interface ITransactionEndpoint
    {
        /// <summary>
        /// Allows to retrieve all transactions available on Coinbase Custody API.
        /// </summary>
        /// <param name="currency">The currency of the transaction, optional parameter</param>
        /// <param name="state">The <see cref="TransactionState"/> of the transaction, optional parameter</param>
        /// <param name="walletId">The wallet which receives the transaction, optional parameter</param>
        /// <param name="type">The <see cref="TransactionType"/> of the transaction, optional parameter.</param>
        /// <param name="startTime">The startTime of the transaction, optional parameter</param>
        /// <param name="endTime">The endTime of the transaction, optional parameter</param>
        /// <param name="before">Request page before (newer than) this pagination id.</param>
        /// <param name="after">Request page after (older than) this pagination id.</param>
        /// <param name="limit">Number of results per request. Maximum 100. Default 25.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="PagedResponse{Transaction}"/> if request succeed.</returns>
        Task<PagedResponse<Transaction>> ListTransactionsAsync(string? currency = null, string? state = null,
            string? walletId = null, string? type = null,
            string? startTime = null, string? endTime = null, string? before = null, string? after = null,
            int? limit = null, CancellationToken cancellationToken = default);


        /// <summary>
        /// Allows you to retrieve a specific transaction if you have the transaction Id.
        /// </summary>
        /// <param name="transactionId">The id of the transaction that we looking for.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="Transaction"/> if request succeed.</returns>
        Task<Transaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default);
    }
}