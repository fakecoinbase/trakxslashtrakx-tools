using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Transaction;

namespace Trakx.Persistence
{
    public class TransactionDataProvider : ITransactionDataProvider
    {
        private readonly IndiceRepositoryContext _dbContext;

        public TransactionDataProvider(IndiceRepositoryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IWrappingTransaction?> GetWrappingTransactionByNativeTransactionHash(string nativeTransactionHash)
        {
            var transaction =
                await _dbContext.WrappingTransactions.FirstOrDefaultAsync(w => w.NativeChainTransactionHash == nativeTransactionHash);

            return transaction;
        }

        public async Task<IWrappingTransaction?> GetWrappingTransactionByEthereumTransactionHash(string ethereumTransactionHash)
        {
            var transaction =
                await _dbContext.WrappingTransactions.FirstOrDefaultAsync(w => w.EthereumTransactionHash == ethereumTransactionHash);

            return transaction;
        }

        public async Task<List<IWrappingTransaction>> GetAllWrappingTransactionsByUser(string userName)
        {
            var transactions = await _dbContext.WrappingTransactions.Where(w => w.User == userName).ToListAsync<IWrappingTransaction>();

            return transactions;
        }

        public async Task<List<IIndiceSupplyTransaction>> GetAllIndiceSupplyTransactionsByUser(string userName)
        {
            var transactions = await _dbContext.IndiceSupplyTransactions.Where(t => t.User == userName)
                .ToListAsync<IIndiceSupplyTransaction>();

            return transactions;
        }

        public async Task<DateTime> GetLastWrappingTransactionDatetime()
        {
            var transaction = await _dbContext.WrappingTransactions.Where(t=>t.TransactionType==TransactionType.Wrap).OrderByDescending(t=>t.TimeStamp).ToListAsync();
            return transaction.Count==0 ? DateTime.MinValue : transaction[0].TimeStamp;
        }
    }
}
