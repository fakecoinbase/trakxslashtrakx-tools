using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Common.Interfaces;

namespace Trakx.Persistence.DAO
{
    public class ExternalAddressDao : IExternalAddress
    {
        #nullable disable
        public ExternalAddressDao() { }
        #nullable restore

        public ExternalAddressDao(IExternalAddress address)
            : this(address.Address, address.CurrencySymbol, 
                address.Balance, address.VerificationAmount,
                address.IsVerified, address.User,
                address.Created, address.LastModified) {}

        public ExternalAddressDao(string address, 
            string currencySymbol, 
            decimal balance = 0,
            decimal? verificationAmount = default,
            bool isVerified = false,
            IUser? user = default, 
            DateTime? created = default,
            DateTime? lastModified = default)
        {
            Balance = balance;
            VerificationAmount = verificationAmount;
            IsVerified = isVerified;
            UserDao = user == default ? default : user as UserDao ?? new UserDao(user);
            Address = address;
            CurrencySymbol = currencySymbol;
            Id = this.GetId();
            Created = created ?? DateTime.UtcNow;
            LastModified = lastModified ?? DateTime.UtcNow;
        }

        public UserDao? UserDao { get; set; }

        #region Implementation of IExternalAddress
        /// <inheritdoc />
        [Column(TypeName = "decimal(38, 18)")]
        public decimal Balance { get; set; }

        /// <inheritdoc />
        [Column(TypeName = "decimal(38, 18)")]
        public decimal? VerificationAmount { get; set; }

        /// <inheritdoc />
        [Key, Required]
        public string Id { get; private set; }

        /// <inheritdoc />
        [NotMapped]
        public IUser? User => UserDao;

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
    }
}
