using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Ardalis.GuardClauses;
using Trakx.Common.Interfaces;

namespace Trakx.Persistence.DAO
{
    public class UserDao : IUser
    {
        #nullable disable
        public UserDao() { }
        #nullable restore

        public UserDao(string userId, 
            List<IDepositorAddress> addresses,
            DateTime? creationDate = default,
            DateTime? lastModified = default)
        {
            Guard.Against.NullOrWhiteSpace(userId, nameof(userId));
            Id = userId;
            AddressDaos = addresses.Select(a => new DepositorAddressDao(a)).ToList();
            var utcNow = DateTime.UtcNow;
            Created = creationDate ?? utcNow;
            LastModified = lastModified ?? utcNow;
        }

        public UserDao(IUser user) 
            : this(user.Id, user.Addresses, user.Created, user.LastModified) { }

        public List<DepositorAddressDao> AddressDaos { get; set; }

        #region Implementation of IUserAddress

        /// <inheritdoc />
        [Key]
        public string Id { get; set; }

        /// <inheritdoc />
        [NotMapped]
        public List<IDepositorAddress> Addresses => AddressDaos.Cast<IDepositorAddress>().ToList();

        #endregion

        #region Implementation of IHasCreatedLastModified

        /// <inheritdoc />
        public DateTime Created { get; set; }

        /// <inheritdoc />
        public DateTime? LastModified { get; set; }

        #endregion
    }
}
