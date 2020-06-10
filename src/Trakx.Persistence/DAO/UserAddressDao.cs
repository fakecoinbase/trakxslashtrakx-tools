using System.ComponentModel.DataAnnotations;
using Trakx.Common.Interfaces;

namespace Trakx.Persistence.DAO
{
    public class UserAddressDao : IUserAddress
    {
        public UserAddressDao(string chainId,string userId,string address)
        {
            Id = $"{UserId}|{ChainId}";
            ChainId = chainId;
            UserId = userId;
            Address = address;
        }

        #region Implementation of IUserAddress

        /// <inheritdoc />
        [Key,Required]
        public string Id { get; set; }

        /// <inheritdoc />
        [Required]
        public string ChainId { get; set; }

        /// <inheritdoc />
        [Required]
        public string UserId { get; set; }

        /// <inheritdoc />
        [Required]
        public string Address { get; set; }
        #endregion
    }
}
