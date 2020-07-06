using System;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;

namespace Trakx.IndiceManager.Server.Data
{
    /// <summary>
    /// This service is used to change update the balances of depositor addresses
    /// based on incoming Coinbase Custody transactions.
    /// </summary>
    public interface IBalanceUpdater : IObserver<CoinbaseTransaction> {}

    /// <inheritdoc cref="IBalanceUpdater" />
    /// <inheritdoc cref="IDisposable" />
    public sealed class BalanceUpdater : IBalanceUpdater, IDisposable
    {
        private readonly IServiceScope _initialisationScope;
        private readonly IDepositorAddressRetriever _addressRetriever;

        /// <inheritdoc />
        public BalanceUpdater(IServiceScopeFactory serviceScopeFactory)
        {
            _initialisationScope = serviceScopeFactory.CreateScope();
            _addressRetriever = _initialisationScope.ServiceProvider.GetService<IDepositorAddressRetriever>();
        }

        #region Implementation of IObserver<in CoinbaseTransaction>

        /// <inheritdoc />
        public void OnCompleted()
        {
            //not implemented yet.
        }

        /// <inheritdoc />
        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnNext(CoinbaseTransaction value)
        {
            ManageRetrievedTransaction(value);
        }

        #endregion

        private void ManageRetrievedTransaction(CoinbaseTransaction transaction)
        {
            if(transaction.ScaledAmount == 0m) return;

            var depositorAddressId = DepositorAddressExtension.GetDepositorAddressId(transaction.Currency, transaction.Source);
            var retrievedAddress = _addressRetriever
                    .GetDepositorAddressById(depositorAddressId)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

            if (retrievedAddress == null)
            {
                var newDepositorAddress = new DepositorAddress(transaction.Source, 
                    transaction.Currency, transaction.ScaledAmount);
                _addressRetriever.AddNewAddress(newDepositorAddress);
            }
            else
            {
                var address = new DepositorAddress(retrievedAddress);
                if (!retrievedAddress.IsVerified
                    && retrievedAddress.User != default
                    && retrievedAddress.VerificationAmount == transaction.ScaledAmount)
                {
                    address.IsVerified = true;
                }

                address.TryUpdateBalance(transaction.ScaledAmount);
                _addressRetriever.UpdateDepositorAddress(address);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _initialisationScope.Dispose();
        }
    }
}
