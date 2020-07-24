using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
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
            List<IExternalAddress> addresses,
            DateTime? creationDate = default,
            DateTime? lastModified = default)
        {
            Guard.Against.NullOrWhiteSpace(userId, nameof(userId));
            Id = userId;
            AddressDaos = addresses.Select(a => new ExternalAddressDao(a)).ToList();
            var utcNow = DateTime.UtcNow;
            Created = creationDate ?? utcNow;
            LastModified = lastModified ?? utcNow;
        }

        public UserDao(IUser user) 
            : this(user.Id, user.Addresses, user.Created, user.LastModified) { }

        [JsonIgnore]
        public List<ExternalAddressDao> AddressDaos { get; set; }

        #region Implementation of IUserAddress

        /// <inheritdoc />
        [Key]
        public string Id { get; set; }

        /// <inheritdoc />
        [NotMapped]
        [JsonIgnore]
        public List<IExternalAddress> Addresses => AddressDaos.Cast<IExternalAddress>().ToList();

        #endregion

        #region Implementation of IHasCreatedLastModified

        /// <inheritdoc />
        public DateTime Created { get; set; }

        /// <inheritdoc />
        public DateTime? LastModified { get; set; }

        #endregion
    }
}
