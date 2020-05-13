using System.Threading.Tasks;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Persistence.DAO;

namespace Trakx.Persistence
{
    public class TransactionDataCreator : ITransactionDataCreator
    {
        private readonly IndiceRepositoryContext _dbContext;

        public TransactionDataCreator(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SaveIndiceSupplyTransaction(IIndiceSupplyTransaction transaction)
        {
            await _dbContext.IndiceSupplyTransactions.AddAsync((IndiceSupplyTransactionDao)transaction);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveWrappingTransaction(IWrappingTransaction transaction)
        {
            await _dbContext.WrappingTransactions.AddAsync((WrappingTransactionDao) transaction);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
