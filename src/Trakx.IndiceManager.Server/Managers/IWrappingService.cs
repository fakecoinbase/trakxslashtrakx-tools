using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Transaction;
using Trakx.IndiceManager.Server.Models;

namespace Trakx.IndiceManager.Server.Managers
{
    /// <summary>
    /// Use this component to retrieve information about Trakx' balance, Trakx addresses, Trakx transactions
    /// from Coinbase custody and to send wrapped/unwrapped tokens.
    /// </summary>
    public interface IWrappingService 
    {
        /// <summary>
        /// Allows to retrieve the Trakx address associated to a specific symbol token.
        /// </summary>
        /// <param name="symbol">The symbol for which we want to know the address.</param>
        /// <returns>The address attached to the <paramref name="symbol"/></returns>
        Task<string> RetrieveAddressFromSymbol(string symbol);


        /// <summary>
        /// Allows to validate a transaction when wrapping tokens or unwrapping tokens.
        /// To validate : must find the transaction and the transaction hash in Coinbase custody
        /// & verify in database that the hash isn't already there to avoid double spend attack.
        /// When we find it, we put the transaction immediately in the database with this hash.
        /// </summary>
        /// <param name="transaction">The transaction that we want to verify.</param>
        /// <returns>The hash of the transaction or null if the hash is not found after a certain period.</returns>
        Task<string> TryToFindTransaction(WrappingTransactionModel transaction);


        /// <summary>
        /// Initiate the wrapping of tokens.
        /// </summary>
        /// <param name="transactionHash">The transaction hash of the transaction made by the user toward the Coinbase custody wallet.
        /// This allows to retrieve all of the information concerning the transaction in the database (amount and receiver address).
        /// </param>
        /// <returns>The hash of the wrapping transaction.</returns>
        Task<string> InitiateWrapping(string transactionHash);

        /// <summary>
        /// Allows to retrieve all the transactions made by a user.
        /// </summary>
        /// <param name="user">The user for who we want the transactions.</param>
        /// <returns>A list a wrapping transaction or null if the user hasn't made any transaction.</returns>
        Task<List<IWrappingTransaction>> GetTransactionByUser(string user);

        /// <summary>
        /// Allows to retrieve Trakx balance, either with native or wrapped tokens.
        /// </summary>
        /// <returns>Return a list of <see cref="AccountBalanceModel"/>.</returns>
        Task<List<AccountBalanceModel>> GetBalances();
    }
}
