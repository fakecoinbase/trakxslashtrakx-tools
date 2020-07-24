using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Represents the user's address mapping.
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
        [JsonIgnore]
        List<IExternalAddress> Addresses { get; }
    }
}
