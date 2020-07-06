using System.Collections.Generic;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Represents represents the user's address mapping.
    /// </summary>
    public interface IUser : IHasCreatedLastModified
    {
        /// <summary>
        /// The Id of the user.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A list of addresses for the current User.
        /// </summary>
        List<IDepositorAddress> Addresses { get; }
    }
}
