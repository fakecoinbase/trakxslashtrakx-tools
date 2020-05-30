using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class TransactionDataModifier : ITransactionDataModifier
    {
        private readonly IndiceRepositoryContext _dbContext;

        public TransactionDataModifier(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ModifyWrappingTransaction(IWrappingTransaction transaction)
        {
            var transactionDao = new WrappingTransactionDao(transaction);

            var retrievedTransaction = await _dbContext.WrappingTransactions.FirstOrDefaultAsync(w =>
                w.EthereumTransactionHash == transactionDao.EthereumTransactionHash ||
                w.NativeChainTransactionHash == transactionDao.NativeChainTransactionHash);

            transactionDao.Id = retrievedTransaction.Id;
            _dbContext.Entry(await _dbContext.WrappingTransactions.FirstOrDefaultAsync(w => w.Id == transactionDao.Id))
                    .CurrentValues.SetValues(transactionDao);
            
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
