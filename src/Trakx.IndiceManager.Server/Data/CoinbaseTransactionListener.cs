using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Interfaces;

namespace Trakx.IndiceManager.Server.Data
{
    /// <summary>
    /// A service used to poll the Coinbase Custody Api to be notified of new deposits
    /// on Trakx' Coinbase Wallets.
    /// </summary>
    public interface ICoinbaseTransactionListener
    {
        /// <summary>
        /// Stream of incoming transaction on Trakx coinbase wallets
        /// </summary>
        IObservable<CoinbaseTransaction> TransactionStream { get; }

        /// <summary> 
        /// Allows to stop the Observable stream by passing a cancellationToken in parameter.
        /// </summary>
        void StopListening();
    }

    /// <inheritdoc cref="ICoinbaseTransactionListener"/>
    /// <inheritdoc cref="IDisposable"/>
    public sealed class CoinbaseTransactionListener : ICoinbaseTransactionListener, IDisposable
    {
        /// <summary>
        /// Interval at which we try to retrieve new transactions from Coinbase Custody.
        /// </summary>
        public static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(10);
        
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger<CoinbaseTransactionListener> _logger;
        private readonly IServiceScope _initialisationScope;
        private readonly ICurrencyCache _currencyCache;

        /// <inheritdoc />
        public CoinbaseTransactionListener(ICoinbaseClient coinbaseClient,
            IServiceScopeFactory serviceScopeFactory,
            ICurrencyCache currencyCache,
            ILogger<CoinbaseTransactionListener> logger,
            IScheduler? scheduler = default)
        {
            _currencyCache = currencyCache;
            _logger = logger;
            _initialisationScope = serviceScopeFactory.CreateScope();
            var transactionDataProvider = _initialisationScope.ServiceProvider.GetService<ITransactionDataProvider>();
            scheduler ??= Scheduler.Default;
            _cancellationTokenSource = new CancellationTokenSource();
            TransactionStream = Observable.Interval(PollingInterval, scheduler)
                .TakeWhile(_ => !_cancellationTokenSource.IsCancellationRequested)
                .Select(async i =>
                {
                    try
                    {
                        logger.LogDebug("Querying Coinbase Custody for new transactions.");
                        var transactions = coinbaseClient.GetTransactions(endTime:
                            await transactionDataProvider.GetLastWrappingTransactionDatetime(),
                            paginationOptions: new PaginationOptions(pageSize:100));
                        var processedTransaction = GetDecimalAmount(transactions);
                        return processedTransaction.ToObservable();
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Failed to get new transactions from Coinbase Custody");
                        return new CoinbaseTransaction[0].ToObservable(scheduler);
                    }
                })
                .SelectMany(t => t)
                .SelectMany(t => t)
                //todo: think about a solution to only do a distinct on a finite set of transactions,
                //it looks like here the only way to do a distinct would be to keep in memory all the
                //transactions that ever appeared in the stream to perform comparison. This is probably
                //slow and leaky.
                .Distinct(t => string.Join("|", t.Hashes))
                .Do(t => logger.LogDebug("New transaction found on wallet {0}", t.WalletId));
        }

        #region Implementation of ICoinbaseTransactionListener

        /// <inheritdoc />
        public void StopListening()
        {
            _logger.LogDebug("Stopping to look for coinbase transactions.");
            _cancellationTokenSource.Cancel();
        }

        /// <inheritdoc />
        public IObservable<CoinbaseTransaction> TransactionStream { get; }

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _initialisationScope?.Dispose();
        }

        #endregion

        private async IAsyncEnumerable<CoinbaseTransaction> GetDecimalAmount(IAsyncEnumerable<CoinbaseRawTransaction> transactionsList)
        {
            await foreach (var transaction in transactionsList)
            {
                var retrievedDecimal = await _currencyCache.GetDecimalsForCurrency(transaction.Currency);
                if (retrievedDecimal.HasValue)
                    yield return new CoinbaseTransaction(transaction, retrievedDecimal.Value);
            }
        }
    }
}
