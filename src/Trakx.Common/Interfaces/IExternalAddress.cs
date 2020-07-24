namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Represents an externally owned address which can be verified by a user
    /// and used for deposits and withdrawals of user funds.
    /// </summary>
    public interface IExternalAddress : IHasCreatedLastModified
    {
        /// <summary>
        /// The balance of the user currently held at Trakx.
        /// </summary>
        decimal Balance { get; }

        /// <summary>
        /// The amount used to verify the address.
        /// </summary>
        decimal? VerificationAmount { get; }

        /// <summary>
        /// The Id of this address.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The User who managed to verify the address.
        /// </summary>
        IUser? User { get; }
        
        /// <summary>
        /// A boolean that represents the address verification. 
        /// </summary>
        bool IsVerified { get; set; }

        /// <summary>
        /// The address to map.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// The symbol of the currency.
        /// </summary>
        string CurrencySymbol { get; }
    }
}