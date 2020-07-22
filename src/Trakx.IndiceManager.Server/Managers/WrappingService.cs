using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Common.Models;

namespace Trakx.IndiceManager.Server.Managers
{
    /// <inheritdoc />
    public class WrappingService : IWrappingService
    {
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly ILogger<WrappingService> _logger;

        /// <inheritdoc />
        public WrappingService(ICoinbaseClient coinbaseClient,
            ILogger<WrappingService> logger)
        {
            _coinbaseClient = coinbaseClient;
            _logger = logger;
        }

        #region Implementation of IWrappingService

        /// <inheritdoc />
        public async Task<string> RetrieveAddressFromSymbol(string symbol)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<string> TryToFindTransaction(WrappingTransactionModel transaction)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<string> InitiateWrapping(string transactionHash)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<List<IWrappingTransaction>> GetTransactionByUser(string user)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<AccountBalanceModel> GetTrakxBalances(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IAsyncEnumerable<Wallet> wallets;
            try
            {
                wallets = _coinbaseClient.GetWallets(cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to retrieve wallets from Coinbase Custody");
                yield break;
            }
            await foreach (var wallet in wallets.WithCancellation(cancellationToken))
            {
                yield return new AccountBalanceModel(wallet.CurrencySymbol, 
                    wallet.Balance, 
                    wallet.UnscaledBalance, 
                    wallet.Name, 
                    wallet.ColdAddress,
                    wallet.UpdatedAt);
            }
        }

        #endregion
    }
}
