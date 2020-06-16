using System;
using System.Collections.Generic;
using System.Text;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Represents represents the user's address mapping.
    /// </summary>
    public interface IUserAddress
    {
        /// <summary>
        /// The Id of the mapping
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The Id of the currency, commonly it's his symbol.
        /// </summary>
        public string ChainId { get; }

        /// <summary>
        /// The  userId representing the owner of the address.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// The address to map.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// The balance of the user currently held at Trakx.
        /// </summary>
        public decimal Balance { get; }

        /// <summary>
        /// The amount used to verify the address.
        /// </summary>
        public decimal VerificationAmount { get; }

        /// <summary>
        /// A boolean that represents the address verification. 
        /// </summary>
        public bool IsVerified { get; }

        /// <summary>
        /// A Datetime that reprensents the creation date of the mapping
        /// </summary>
        public DateTime CreationDate { get; }
    }

    public static class UserAddressExtension
    {
        public static string GetId(this IUserAddress userAddress)
        {
            return $"{userAddress.UserId}|{userAddress.ChainId}";
        }
    }
}
