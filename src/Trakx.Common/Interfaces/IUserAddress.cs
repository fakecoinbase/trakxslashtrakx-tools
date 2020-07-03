using System;

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
        string Id { get; }

        /// <summary>
        /// The Id of the currency, commonly it's his symbol.
        /// </summary>
        string ChainId { get; }

        /// <summary>
        /// The  userId representing the owner of the address.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// The address to map.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// The balance of the user currently held at Trakx.
        /// </summary>
        decimal Balance { get; }

        /// <summary>
        /// The amount used to verify the address.
        /// </summary>
        decimal VerificationAmount { get; }

        /// <summary>
        /// A boolean that represents the address verification. 
        /// </summary>
        bool IsVerified { get; }

        /// <summary>
        /// A Datetime that represents the creation date of the mapping
        /// </summary>
        DateTime CreationDate { get; }

        /// <summary>
        /// A Datetime that represents the last transaction update.
        /// </summary>
        DateTime LastUpdate { get; }
    }

    public static class UserAddressExtension
    {
        public static string GetId(this IUserAddress userAddress)
        {
            return $"{userAddress.UserId}|{userAddress.ChainId}";
        }
    }
}
