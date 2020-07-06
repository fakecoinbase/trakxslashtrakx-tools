using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trakx.IndiceManager.Server.Data;

namespace Trakx.IndiceManager.Server
{
    /// <summary>
    /// Long running tasks in charge of listening to events about new deposits on Coinbase
    /// Custody wallets, and update internal ledger balances in consequence.
    /// </summary>
    /// <inheritdoc cref="BackgroundService"/>
    public class BalanceUpdaterService : BackgroundService
    {
        private readonly IBalanceUpdater _balanceUpdater;
        private readonly ILogger<BalanceUpdaterService> _logger;
        private readonly ICoinbaseTransactionListener _coinbaseTransactionListener;
        private IDisposable? _balanceUpdateSubscription;

        /// <inheritdoc />
        public BalanceUpdaterService(ICoinbaseTransactionListener coinbaseTransactionListener, 
            IBalanceUpdater balanceUpdater, ILogger<BalanceUpdaterService> logger)
        {
            _balanceUpdater = balanceUpdater;
            _coinbaseTransactionListener = coinbaseTransactionListener;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _balanceUpdateSubscription = _coinbaseTransactionListener
                .TransactionStream.Subscribe(_balanceUpdater);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken).ConfigureAwait(false);
            }
        }

        #region Overrides of BackgroundService

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            _balanceUpdateSubscription?.Dispose();
        }

        #endregion
    }
}
