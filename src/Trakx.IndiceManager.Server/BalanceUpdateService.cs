using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trakx.IndiceManager.Server.Data;

namespace Trakx.IndiceManager.Server
{
    public class BalanceUpdateService : BackgroundService
    {
        private readonly IUserBalanceUpdater _balanceUpdater;
        private readonly ILogger<BalanceUpdateService> _logger;
        private readonly ICoinbaseTransactionListener _coinbaseTransactionListener;
        private IDisposable _balanceUpdateSubscription;

        public BalanceUpdateService(ICoinbaseTransactionListener coinbaseTransactionListener, 
            IUserBalanceUpdater balanceUpdater, ILogger<BalanceUpdateService> logger)
        {
            _balanceUpdater = balanceUpdater;
            _coinbaseTransactionListener = coinbaseTransactionListener;
            _logger = logger;
        }
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
            _balanceUpdateSubscription.Dispose();
        }

        #endregion
    }
}
