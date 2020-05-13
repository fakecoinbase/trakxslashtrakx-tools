using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Transaction;
using Trakx.IndiceManager.Server.Models;

namespace Trakx.IndiceManager.Server.Managers
{
    /// <summary>
    /// Use this component to interact with the database to retrieve and save transaction
    /// made by issuing or redeeming indices.
    /// </summary>
    public interface IIndiceSupplyService
    {
        /// <summary>
        /// This function allows to save transaction in the database when issuing or redeeming indices.
        /// </summary>
        /// <param name="transactionToSave">The transaction that we want to save.</param>
        /// <returns>True if the adding succeed, false else.</returns>
        Task<bool> TryToSaveTransaction(IndiceSupplyTransactionModel transactionToSave);

        /// <summary>
        /// This function tries to retrieve all of the issuing and redeeming of indices made by a specific user.
        /// </summary>
        /// <param name="name">The user that made the transactions.</param>
        /// <returns>A list of <see cref="IIndiceSupplyTransaction"/> with all the transactions.</returns>
        Task<List<IIndiceSupplyTransaction>> GetAllTransactionByUser(string name);
    }
}
