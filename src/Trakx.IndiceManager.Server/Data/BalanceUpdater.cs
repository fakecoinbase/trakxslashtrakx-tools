using System;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;

namespace Trakx.IndiceManager.Server.Data
{
    public interface IUserBalanceUpdater : IObserver<Transaction>
    {
        /// <summary>
        /// Updates the user balance upon incoming new transactions.
        /// </summary>
        /// <param name="transaction">The new transaction to be processed.</param>
        /// <param name="retrievedUser">The user who made the transaction.</param>
        /// <returns>True if a balance was updated, false otherwise.</returns>
        bool TryUpdateUserBalance(IUserAddress retrievedUser, Transaction transaction);
    }

    /// <inheritdoc />
    public class UserBalanceUpdater : IUserBalanceUpdater
    {
        private readonly IUserAddressProvider _userAddressProvider;

        public UserBalanceUpdater(IUserAddressProvider userAddressProvider)
        {
            _userAddressProvider = userAddressProvider;
        }


        #region Implementation of IUserBalanceUpdater
        /// <inheritdoc />
        public bool TryUpdateUserBalance(IUserAddress retrievedUser, Transaction transaction)
        {
            var retrievedUserDao = new UserAddressDao(retrievedUser);
            if (transaction.Amount == retrievedUserDao.VerificationAmount)
            {
                 return _userAddressProvider.ValidateMappingAddress(retrievedUserDao).GetAwaiter().GetResult();
            }
            retrievedUserDao.Balance += transaction.Amount;
            return _userAddressProvider.UpdateUserBalance(retrievedUserDao).GetAwaiter().GetResult();
        }

        #endregion

        #region Implementation of IObserver<in Transaction>

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
        public void OnNext(Transaction transaction)
        {
            ManageRetrievedTransaction(transaction);
        }
        #endregion

        private void ManageRetrievedTransaction(Transaction transaction)
        {
            var retrievedUser =
                _userAddressProvider.TryToGetUserAddressByAddress(transaction.Source).ConfigureAwait(false).GetAwaiter().GetResult();

            if (retrievedUser != null)
            {
                TryUpdateUserBalance(retrievedUser, transaction);
            }
            else
            {
                var newUserAddress = new UserAddress(transaction.Currency, transaction.Source, transaction.Source, 0, DateTime.Now,false, transaction.Amount);
                _userAddressProvider.AddNewMapping(newUserAddress);
            }
        }


    }
}
