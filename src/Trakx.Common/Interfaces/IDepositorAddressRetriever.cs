using System.Threading;
using System.Threading.Tasks;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Use this interface to interact with DepositorAddressDao objects and the database.
    /// </summary>
    public interface IDepositorAddressRetriever
    {
        /// <summary>
        /// Tries to update an existing depositor address.
        /// </summary>
        /// <param name="address">The instance of address used to update the existing address.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>True if the balance is modified, false otherwise.</returns>
        Task<bool> UpdateDepositorAddress(IDepositorAddress address, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to add a new depositor address in the database.
        /// </summary>
        /// <param name="addressToSave">The depositor address that we want to put in the database.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>True if the addition succeed, false otherwise.</returns>
        Task<bool> AddNewAddress(IDepositorAddress addressToSave, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to return the depositorAddress associated to a specific id.
        /// </summary>
        /// <param name="depositorAddressId">The Id of the depositor address.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="IDepositorAddress"/> or null if it doesn't exist.</returns>
        Task<IDepositorAddress?> GetDepositorAddressById(string depositorAddressId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Call this method when a user tries to initiate a new mapping. This will set a random verification
        /// amount on the deposit address and associate the user to the address.
        /// Only when a transaction with the correct amount is sent from the deposit address will the
        /// address get verified. See balance updater in Coinbase Custody client project.
        /// </summary>
        /// <param name="claimedAddress">The address for which a user is claiming ownership.</param>
        /// <param name="candidate">The user claiming he owns a deposit address.</param>
        /// <param name="decimals">The number of decimals associated with the <see cref="IDepositorAddress.CurrencySymbol" /></param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>True is modification succeed, false otherwise.</returns>
        Task<bool> AssociateCandidateUser(IDepositorAddress claimedAddress, IUser candidate, int decimals,
            CancellationToken cancellationToken = default);
    }
}
