using System;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.IndiceManager.Server.Data
{
    public interface IUserBalanceUpdater : IObserver<Transaction>
    {
        /// <summary>
        /// Updates the user balance upon incoming new transactions.
        /// </summary>
        /// <param name="transaction">The new transaction to be processed.</param>
        /// <returns>True if a balance was updated, false otherwise.</returns>
        bool TryUpdateUserBalance(Transaction transaction);
    }

    /// <inheritdoc />
    public class UserBalanceUpdater : IUserBalanceUpdater
    {
        #region Implementation of IUserBalanceUpdater

        public UserBalanceUpdater()
        {
            //useraddressprovider
        }

        /// <inheritdoc />
        public bool TryUpdateUserBalance(Transaction transaction)
        {
            //database update ... 
            return false;
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
            if (transaction.Source == "aaaa" && transaction.Amount == 123)
                TryUpdateUserBalance(transaction);
        }

        #endregion
    }
}
