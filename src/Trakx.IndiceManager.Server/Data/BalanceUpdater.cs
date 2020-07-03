using System;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;

namespace Trakx.IndiceManager.Server.Data
{
    public interface IUserBalanceUpdater : IObserver<ProcessedTransaction>
    {
        /// <summary>
        /// Updates the user balance upon incoming new transactions.
        /// </summary>
        /// <param name="transaction">The new transaction to be processed.</param>
        /// <param name="retrievedUser">The user who made the transaction.</param>
        /// <returns>True if a balance was updated, false otherwise.</returns>
        bool TryUpdateUserBalance(IUserAddress retrievedUser, ProcessedTransaction transaction);
    }

    /// <inheritdoc cref="IUserBalanceUpdater" />
    public class UserBalanceUpdater : IUserBalanceUpdater, IDisposable
    {
        private readonly IUserAddressProvider _userAddressProvider;
        private readonly IServiceScope _initialisationScope;

        public UserBalanceUpdater(IServiceScopeFactory serviceScopeFactory)
        {
            _initialisationScope = serviceScopeFactory.CreateScope();
            _userAddressProvider = _initialisationScope.ServiceProvider.GetService<IUserAddressProvider>();
        }


        #region Implementation of IUserBalanceUpdater
        /// <inheritdoc />
        public bool TryUpdateUserBalance(IUserAddress retrievedUser, ProcessedTransaction transaction)
        {
            var retrievedUserDao = new UserAddressDao(retrievedUser);
            if (transaction.DecimalAmount == retrievedUserDao.VerificationAmount && !retrievedUserDao.IsVerified)
            {
                retrievedUserDao.LastUpdate = transaction.CreatedAt.DateTime;
                return _userAddressProvider.ValidateMappingAddress(retrievedUserDao).GetAwaiter().GetResult();
            }
            retrievedUserDao.Balance += transaction.DecimalAmount;
            retrievedUserDao.LastUpdate = transaction.CreatedAt.DateTime;
            return _userAddressProvider.UpdateUserBalance(retrievedUserDao).GetAwaiter().GetResult();
        }

        #endregion

        #region Implementation of IObserver<in ProcessedTransaction>

        /// <inheritdoc />
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnNext(ProcessedTransaction value)
        {
            ManageRetrievedTransaction(value);
        }
        #endregion

        private void ManageRetrievedTransaction(ProcessedTransaction transaction)
        {
            var retrievedUser =
                _userAddressProvider.TryToGetUserAddressByAddress(transaction.Source)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

            if (retrievedUser != null)
            {
                TryUpdateUserBalance(retrievedUser, transaction);
            }
            else
            {
                var newUserAddress = new UserAddress(transaction.Currency, transaction.Source, 0, 
                    DateTime.Now, balance: transaction.Amount)
                { LastUpdate = transaction.CreatedAt.DateTime };
                _userAddressProvider.AddNewMapping(newUserAddress);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _initialisationScope.Dispose();
        }
    }
}
