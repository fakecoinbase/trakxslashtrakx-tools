using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces;

namespace Trakx.IndiceManager.Server.Data
{
    public interface ICoinbaseTransactionListener
    {
        /// <summary>
        /// Stream of incoming transaction on Trakx coinbase wallets
        /// </summary>
        IObservable<Transaction> TransactionStream { get; }

        /// <summary>
        /// Allows to stop the Observable stream by passing a cancellationToken in parameter.
        /// </summary>
        /// <param name="cancellationToken">The CancellationToken which takes in parameter the time at which we have to stop the execution.</param>
        void StopListening();
    }

    /// <inheritdoc />
    public class CoinbaseTransactionListener : ICoinbaseTransactionListener, IDisposable
    {
        public static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(10);
        private readonly CancellationTokenSource _cancellationTokenSource ;
        private readonly ITransactionDataProvider _transactionDataProvider;

        public CoinbaseTransactionListener(
            ICoinbaseClient coinbaseClient, 
            ILogger<CoinbaseTransactionListener> logger, ITransactionDataProvider transactionDataProvider, IScheduler scheduler = default)
        {
            _transactionDataProvider = transactionDataProvider;
            scheduler ??= Scheduler.Default;
            _cancellationTokenSource = new CancellationTokenSource();
            TransactionStream = Observable.Interval(PollingInterval, scheduler)
                .TakeWhile(_ => !_cancellationTokenSource.IsCancellationRequested)
                .Select(async i =>
                {
                    try
                    {
                        logger.LogDebug("Querying Coinbase Custody for new transactions.");
                        var page = await coinbaseClient
                            .ListTransactionsAsync(startTime:
                                (await _transactionDataProvider.GetLastWrappingTransactionDatetime()).ToIso8601())
                            .ConfigureAwait(false);
                        var transactions = page.Data;
                        return transactions.ToObservable();
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Failed to get new transactions from Coinbase Custody");
                        return new Transaction[0].ToObservable(scheduler);
                    }
                })
                .SelectMany(t => t)
                .SelectMany(t => t)
                .Distinct(t => string.Join("", t.Hashes))
                .Do(t => logger.LogDebug("New transaction found on wallet {0}", t.WalletId));
        }

        #region Implementation of ICoinbaseTransactionListener

        /// <inheritdoc />
        public void StopListening()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <inheritdoc />
        public IObservable<Transaction> TransactionStream { get; }

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        #endregion
    }
}
