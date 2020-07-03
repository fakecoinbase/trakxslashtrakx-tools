using System;
using System.Threading.Tasks;
using Trakx.Common.Interfaces;

namespace  Trakx.Common.Interfaces
{
    /// <summary>
    /// Use this interface to interact with UserAddressDao objects and the database.
    /// </summary>
    public interface IUserAddressProvider
    {
        /// <summary>
        /// Try to return the UserAddress associated to a specific address.
        /// </summary>
        /// <param name="address">The address of the mapping.</param>
        /// <returns>A <see cref="IUserAddress"/> if it exists, null else.</returns>
        Task<IUserAddress?> TryToGetUserAddressByAddress(string address);

        /// <summary>
        /// Try to validate a specific mapping.
        /// </summary>
        /// <param name="userAddressToValidate">The userAddress to validate.</param>
        /// <returns>True if the mapping is validate, false else.</returns>
        Task<bool> ValidateMappingAddress(IUserAddress userAddressToValidate);

        /// <summary>
        /// Try to update the balance of a user.
        /// </summary>
        /// <param name="userAddress">The userAddress for which we want to change the balance.</param>
        /// <returns>True if the balance is modified, false else.</returns>
        Task<bool> UpdateUserBalance(IUserAddress userAddress);

        /// <summary>
        /// Try to add a new mapping in the database.
        /// </summary>
        /// <param name="userAddressToSave">The UserAddress that we want to put in the database.</param>
        /// <returns>True if the addition succeed, false else.</returns>
        Task<bool> AddNewMapping(IUserAddress userAddressToSave);

        /// <summary>
        /// Try to get the last transaction date.
        /// </summary>
        /// <returns>The last date at which Coinbase Custody Api was requested.</returns>
        DateTime GetLastTransactionDate();
    }
}