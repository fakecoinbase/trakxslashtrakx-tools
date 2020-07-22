using System.Collections.Generic;
using System.Threading.Tasks;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Common.Models;
using IndiceSupplyTransactionModel = Trakx.IndiceManager.Server.Models.IndiceSupplyTransactionModel;

namespace Trakx.IndiceManager.Server.Managers
{
    /// <inheritdoc />
    public class IndiceSupplyService :IIndiceSupplyService
    {
        private readonly ITransactionDataProvider _transactionDataProvider;
        private readonly ITransactionDataCreator _transactionDataCreator;

        public IndiceSupplyService(ITransactionDataCreator transactionDataCreator, ITransactionDataProvider transactionDataProvider)
        {
            _transactionDataProvider = transactionDataProvider;
            _transactionDataCreator = transactionDataCreator;
        }

        /// <inheritdoc />
        public async Task<bool> TryToSaveTransaction(IndiceSupplyTransactionModel transactionToSave)
        {
            var supplyTransaction = transactionToSave.ConvertToIIndiceSupplyTransaction();
            return await _transactionDataCreator.SaveIndiceSupplyTransaction(supplyTransaction).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<List<IIndiceSupplyTransaction>> GetAllTransactionByUser(string userName)
        {
            return await _transactionDataProvider.GetAllIndiceSupplyTransactionsByUser(userName).ConfigureAwait(false);
        }
    }
}
