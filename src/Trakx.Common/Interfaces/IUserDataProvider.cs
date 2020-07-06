using System.Threading;
using System.Threading.Tasks;

namespace  Trakx.Common.Interfaces
{
    /// <summary>
    /// Use this interface to interact with UserDao objects and the database.
    /// </summary>
    public interface IUserDataProvider
    {
        /// <summary>
        /// Tries to return the User associated to a specific Id.
        /// </summary>
        /// <param name="userId">The Id of the user.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A <see cref="IUser"/> if it exists, null otherwise.</returns>
        Task<IUser?> TryGetUserById(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to add a new user in the database.
        /// </summary>
        /// <param name="userToSave">The UserAddress that we want to put in the database.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>True if the addition succeed, false otherwise.</returns>
        Task<bool> TryAddNewUser(IUser userToSave, CancellationToken cancellationToken = default);
    }
}