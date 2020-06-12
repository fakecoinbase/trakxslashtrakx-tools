using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Allows to retrieve transactions data in the database. Any type of transactions, whether wrappingTransactions or indicesSupplyTransactions.
    /// </summary>
    public interface ITransactionDataProvider
    {
        /// <summary>
        /// Allows to retrieve a <see cref="IWrappingTransaction"/> by the nativeTransactionHash in order to perform wrapping
        /// and to verify that the transaction associated to a hash is unique in the database.
        /// </summary>
        /// <param name="nativeTransactionHash">The hash of the transaction, on the native chain, that we are looking for.</param>
        /// <returns>A <see cref="IWrappingTransaction"/> if it's in the database, null otherwise.</returns>
        Task<IWrappingTransaction?> GetWrappingTransactionByNativeTransactionHash(string nativeTransactionHash);

        /// <summary>
        /// Allows to retrieve a <see cref="IWrappingTransaction"/> by the ethereumTransactionHash in order to perform UnWrapping
        /// and to verify that the transaction associated to a hash is unique in the database.
        /// </summary>
        /// <param name="ethereumTransactionHash">The hash of the transaction, on the ethereum mainnet, that we are looking for.</param>
        /// <returns>A <see cref="IWrappingTransaction"/> if it's in the database, null otherwise.</returns>
        Task<IWrappingTransaction?> GetWrappingTransactionByEthereumTransactionHash(string ethereumTransactionHash);

        /// <summary>
        /// Allows to retrieve all of the wrapping transactions made by a specific user.
        /// </summary>
        /// <param name="userName">The name of the user for who we're looking for the transactions.</param>
        /// <returns>A list of <see cref="IWrappingTransaction"/>.</returns>
        Task<List<IWrappingTransaction>> GetAllWrappingTransactionsByUser(string userName);

        /// <summary>
        /// Allows to retrieve all of the indiceSupply Transactions made by a specific user.
        /// </summary>
        /// <param name="userName">The name of the user for who we're looking for the transactions.</param>
        /// <returns>A list of <see cref="IIndiceSupplyTransaction"/>.</returns>
        Task<List<IIndiceSupplyTransaction>> GetAllIndiceSupplyTransactionsByUser(string userName);

        /// <summary>
        /// Allows to retrieve the last wrapping transaction made.
        /// </summary>
        /// <returns></returns>
        Task<DateTime> GetLastWrappingTransactionDatetime();
    }
}