using System.Threading.Tasks;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Allows to save transactions in the database. Any type of transactions, whether wrappingTransactions or indicesSupplyTransactions.
    /// </summary>
    public interface ITransactionDataCreator
    {
        /// <summary>
        /// Allows to save an <see cref="IIndiceSupplyTransaction"/> in the database.
        /// </summary>
        /// <param name="transaction">The transaction that we want to save.</param>
        /// <returns>A boolean : true if the addition succeed. False otherwise.</returns>
        Task<bool> SaveIndiceSupplyTransaction(IIndiceSupplyTransaction transaction);

        /// <summary>
        /// Allows to save an <see cref="IWrappingTransaction"/> in the database.
        /// </summary>
        /// <param name="transaction">The transaction that we want to save.</param>
        /// <returns>A boolean : true if the addition succeed. False otherwise.</returns>
        Task<bool> SaveWrappingTransaction(IWrappingTransaction transaction);
    }
}