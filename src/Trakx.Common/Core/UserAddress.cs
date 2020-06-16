using System;
using System.Collections.Generic;
using System.Text;
using Trakx.Common.Interfaces;

namespace Trakx.Common.Core
{
    public class UserAddress: IUserAddress
    {
        public UserAddress(string chainId, string userId, string address, decimal verificationAmount,DateTime creationDate, bool isVerified=false,decimal balance=0)
        {
            ChainId = chainId;
            UserId = userId;
            Address = address;
            VerificationAmount = verificationAmount;
            IsVerified = isVerified;
            Balance = balance;
            Id = this.GetId();
            CreationDate = creationDate;
        }

        #region Implementation of IUserAddress

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string ChainId { get; }

        /// <inheritdoc />
        public string UserId { get; }

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

        #endregion
    }
}
