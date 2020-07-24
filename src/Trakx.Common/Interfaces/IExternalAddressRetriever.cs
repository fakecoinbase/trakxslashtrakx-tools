using System.Threading;
using System.Threading.Tasks;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Use this interface to interact with ExternalAddressDao objects and the database.
    /// </summary>
    public interface IExternalAddressRetriever
    {
        /// <summary>
        /// Tries to update an existing external address.
        /// </summary>
        /// <param name="addressToModify">The instance of address used to update the existing address.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>True if the balance is modified, false otherwise.</returns>
        Task<bool> UpdateExternalAddress(IExternalAddress addressToModify, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to add a new external address in the database.
        /// </summary>
        /// <param name="addressToSave">The external address that we want to put in the database.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>True if the addition succeed, false otherwise.</returns>
        Task<bool> AddNewAddress(IExternalAddress addressToSave, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to return the externalAddress associated to a specific id.
        /// </summary>
        /// <param name="externalAddressId">The Id of the external address.</param> 
        /// <param name="includeUser">A boolean to retrieve user or not.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param> 
        /// <returns>A <see cref="IExternalAddress"/> or null if it doesn't exist.</returns>
        Task<IExternalAddress?> GetExternalAddressById(string externalAddressId, bool includeUser = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Call this method when a user tries to initiate a new mapping. This will set a random verification
        /// amount on the external address and associate the user to the address.
        /// Only when a transaction with the correct amount is sent from the external address will the
        /// address get verified. See balance updater in Coinbase Custody client project.
        /// </summary>
        /// <param name="claimedAddress">The address for which a user is claiming ownership.</param>
        /// <param name="candidate">The user claiming he owns an external address.</param>
        /// <param name="decimals">The number of decimals associated with the <see cref="IExternalAddress.CurrencySymbol" /></param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>True is modification succeed, false otherwise.</returns>
        Task<bool> AssociateCandidateUser(IExternalAddress claimedAddress, IUser candidate, int decimals,
            CancellationToken cancellationToken = default);
    }
}
