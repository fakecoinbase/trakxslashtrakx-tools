using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Common.Interfaces;

namespace Trakx.Persistence.DAO
{
    public class UserAddressDao : IUserAddress
    {
        public UserAddressDao(string chainId,string userId,string address,decimal verificationAmount,DateTime creationDate,decimal balance=0,bool isVerified=false)
        {
            IsVerified = isVerified;
            Balance = balance;
            ChainId = chainId;
            UserId = userId;
            Address = address;
            VerificationAmount = verificationAmount;
            CreationDate = creationDate;
            Id = this.GetId();
        }

        public UserAddressDao(IUserAddress userAddress)
        {
            IsVerified = userAddress.IsVerified;
            Balance = userAddress.Balance;
            ChainId = userAddress.ChainId;
            UserId = userAddress.UserId;
            Address = userAddress.Address;
            VerificationAmount = userAddress.VerificationAmount;
            CreationDate = userAddress.CreationDate;
            Id = userAddress.Id;
        }

        #region Implementation of IUserAddress

        /// <inheritdoc />
        [Required]
        public string Id { get; private set; }

        /// <inheritdoc />
        [Required]
        public string ChainId { get; set; }

        /// <inheritdoc />
        [Required]
        public string UserId { get; set; }

        /// <inheritdoc />
        [Key, Required]
        public string Address { get; set; }

        /// <inheritdoc />
        [Column(TypeName = "decimal(38, 18)")]
        public decimal Balance { get; set; }

        /// <inheritdoc />
        [Column(TypeName = "decimal(38, 18)")]
        public decimal VerificationAmount { get; set; }

        /// <inheritdoc />
        public bool IsVerified { get; set; }

        /// <inheritdoc />
        public DateTime CreationDate { get; set; }

        #endregion
    }
}
