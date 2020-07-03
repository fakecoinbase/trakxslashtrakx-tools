using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
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
        IObservable<ProcessedTransaction> TransactionStream { get; }

        /// <summary> 
        /// Allows to stop the Observable stream by passing a cancellationToken in parameter.
        /// </summary>
        void StopListening();
    }

    /// <inheritdoc cref="ICoinbaseTransactionListener"/>
    /// <inheritdoc cref="IDisposable"/>
    public class CoinbaseTransactionListener : ICoinbaseTransactionListener, IDisposable
    {
        public static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(10);
        private readonly CancellationTokenSource _cancellationTokenSource ;
        private readonly ITransactionDataProvider _transactionDataProvider;
        private readonly IUserAddressProvider _userAddressProvider;
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CoinbaseTransactionListener> _logger;
        private readonly IServiceScope _initialisationScope;

        public CoinbaseTransactionListener(
            ICoinbaseClient coinbaseClient, 
            ILogger<CoinbaseTransactionListener> logger, 
            IServiceScopeFactory serviceScopeFactory, IMemoryCache memoryCache,
            IScheduler scheduler = default)
        {
            _cache = memoryCache;
            _logger = logger;
            _coinbaseClient = coinbaseClient;
            _initialisationScope = serviceScopeFactory.CreateScope();
            _transactionDataProvider = _initialisationScope.ServiceProvider.GetService<ITransactionDataProvider>();
            _userAddressProvider = _initialisationScope.ServiceProvider.GetService<IUserAddressProvider>();
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
                                 _userAddressProvider.GetLastTransactionDate().ToIso8601(),limit:100)
                            .ConfigureAwait(false);
                        var transactions = page.Data;
                        var finalTransactions = transactions.ToList();
                        while (transactions.Length == 100)
                        {
                             page = await coinbaseClient
                                .ListTransactionsAsync(startTime:
                                    _userAddressProvider.GetLastTransactionDate().ToIso8601(), limit: 100,after:page.Pagination.After)
                                .ConfigureAwait(false);
                             transactions = page.Data;
                             foreach (var transaction in transactions)
                             {
                                 finalTransactions.Add(transaction);
                             }
                        }
                        return GetDecimalAmount(finalTransactions).ToObservable();
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Failed to get new transactions from Coinbase Custody");
                        return new ProcessedTransaction[0].ToObservable(scheduler);
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
        public IObservable<ProcessedTransaction> TransactionStream { get; }

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            _initialisationScope.Dispose();
        }

        #endregion

        private IEnumerable<ProcessedTransaction> GetDecimalAmount(List<Transaction> transactionsList)
        {
            try
            {
                return transactionsList.Select(transaction => new ProcessedTransaction((long) GetOrCreateFromCache(transaction.Currency), transaction)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get decimal amounts for transactions from Coinbase Custody");
                throw;
            }
        }

        private object GetOrCreateFromCache(string symbol)
        {
            if (_cache.TryGetValue("coinbase_decimal_"+symbol, out var cacheEntry)) return cacheEntry;
           
            cacheEntry = _coinbaseClient.GetCurrencyAsync(symbol).GetAwaiter().GetResult().Decimals;

            _cache.Set("coinbase_decimal_"+symbol,cacheEntry,TimeSpan.FromDays(1));
            return cacheEntry;
        }
    }
}
