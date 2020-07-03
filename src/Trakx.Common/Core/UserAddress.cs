using System;
using Trakx.Common.Interfaces;

namespace Trakx.Common.Core
{
    public class UserAddress : IUserAddress
    {
        public UserAddress(string chainId, string address, decimal verificationAmount,
            DateTime creationDate, string? userId = default, bool isVerified = false, decimal balance = 0)
        {
            ChainId = chainId;
            Address = address;
            VerificationAmount = verificationAmount;
            CreationDate = creationDate;
            UserId = userId;
            IsVerified = isVerified;
            Balance = balance;
            Id = this.GetId();
        }

        #region Implementation of IUserAddress

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string ChainId { get; }

        /// <inheritdoc />
        public string? UserId { get; }

        /// <inheritdoc />
        public string Address { get; }

        /// <inheritdoc />
        public decimal Balance { get; set; }

        /// <inheritdoc />
        public decimal VerificationAmount { get; }

        /// <inheritdoc />
        public bool IsVerified { get; }

        /// <inheritdoc />
        public DateTime CreationDate { get; }

        /// <inheritdoc />
        public DateTime LastUpdate { get; set; }

        #endregion
    }
}
