using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using Trakx.Common.Interfaces;

namespace Trakx.Common.Core
{
    public class User : IUser
    {
        #region Implementation of IUserAddress

        public User(string userId, List<IDepositorAddress> addresses, 
            DateTime? created = default,
            DateTime? lastModified = default)
        {
            Guard.Against.NullOrWhiteSpace(userId, nameof(userId));
            Id = userId;
            Addresses = addresses;
            var utcNow = DateTime.UtcNow;
            Created = created ?? utcNow;
            LastModified = lastModified ?? utcNow;
        }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public List<IDepositorAddress> Addresses { get; }

        #endregion

        #region Implementation of IHasCreatedLastModified

        /// <inheritdoc />
        public DateTime Created { get; set; }

        /// <inheritdoc />
        public DateTime? LastModified { get; set; }

        #endregion
    }
}
