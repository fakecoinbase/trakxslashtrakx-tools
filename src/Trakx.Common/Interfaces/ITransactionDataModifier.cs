using System.Threading.Tasks;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Allows to modify transactions in the database. Any type of transactions, whether wrappingTransactions or indicesSupplyTransactions.
    /// </summary>
    public interface ITransactionDataModifier
    {
        /// <summary>
        /// Allows to modify an <see cref="IWrappingTransaction"/> in the database.
        /// </summary>
        /// <param name="transaction">The transaction that we want to modify.</param>
        /// <returns>A boolean : true if the addition succeed. False otherwise.</returns>
        Task<bool> ModifyWrappingTransaction(IWrappingTransaction transaction);
    }
}