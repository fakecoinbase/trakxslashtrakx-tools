using System;
using Ardalis.GuardClauses;
using Trakx.Common.Interfaces;

namespace Trakx.Common.Core
{
    public class ExternalAddress : IExternalAddress
    {
        private readonly object _balanceLock = new object();

        public ExternalAddress(string address,
            string currencySymbol,
            decimal balance = 0,
            decimal? verificationAmount = default,
            bool isVerified = false,
            IUser? user = default,
            DateTime? created = default,
            DateTime? lastModified = default)
        {
            Guard.Against.Negative(balance, nameof(balance));
            Balance = balance;
            VerificationAmount = verificationAmount;
            IsVerified = isVerified;
            Address = address;
            User = user;
            CurrencySymbol = currencySymbol;
            Id = this.GetId();
            var now = DateTime.UtcNow;
            Created = created ?? now;
            LastModified = lastModified ?? now;
        }

        public ExternalAddress(IExternalAddress address, decimal? newBalance = default)
            : this(address.Address,
                address.CurrencySymbol,
                newBalance ?? address.Balance,
                address.VerificationAmount,
                address.IsVerified,
                address.User,
                address.Created,
                address.LastModified)
        { }

        #region Implementation of IExternalAddress
        /// <inheritdoc />
        public decimal Balance { get; private set; }

        /// <inheritdoc />
        public decimal? VerificationAmount { get; set; }

        /// <inheritdoc />
        public string Id { get; private set; }

        /// <inheritdoc />
        public IUser? User { get; set; }

        /// <inheritdoc />
        public bool IsVerified { get; set; }

        /// <inheritdoc />
        public string Address { get; set; }

        /// <inheritdoc />
        public string CurrencySymbol { get; set; }

        #endregion

        #region Implementation of IHasCreatedLastModified

        /// <inheritdoc />
        public DateTime Created { get; set; }

        /// <inheritdoc />
        public DateTime? LastModified { get; set; }

        #endregion

        public bool TryUpdateBalance(decimal balanceChange)
        {
            lock (_balanceLock)
            {
                var newBalance = Balance + balanceChange;
                var updateIsValid = newBalance > 0;

                if (!updateIsValid) return false;

                Balance = newBalance;
                LastModified = DateTime.UtcNow;

                return true;
            }
        }
    }
}
