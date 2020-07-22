using System;
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
        /// <param name="paginationOptions">Optional custom pagination options. Allows to start searching from a custom point
        /// in time, or to change default number of results returned per page.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="PagedResponse{CoinbaseTransaction}"/> if request succeed.</returns>
        Task<PagedResponse<CoinbaseTransaction>> ListTransactionsAsync(string? currency = null, TransactionState? state = null,
            string? walletId = null, TransactionType? type = null,
            DateTime? startTime = null, DateTime? endTime = null, 
            PaginationOptions? paginationOptions = default, CancellationToken cancellationToken = default);


        /// <summary>
        /// Allows you to retrieve a specific transaction if you have the transaction Id.
        /// </summary>
        /// <param name="transactionId">The id of the transaction that we looking for.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="CoinbaseTransaction"/> if request succeed.</returns>
        Task<CoinbaseTransaction> GetTransactionAsync(string transactionId, CancellationToken cancellationToken = default);
    }
}