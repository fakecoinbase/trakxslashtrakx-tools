using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests.Model;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests.Unit
{
    [Collection(nameof(EmptyDbContextCollection))]
    public sealed class TransactionDataModifierTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly ITransactionDataModifier _transactionDataModifier;
        private readonly MockCreator _mockCreator;

        public TransactionDataModifierTests(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _transactionDataModifier = new TransactionDataModifier(_context);
            _mockCreator = new MockCreator(output);
        }

        [Fact]
        public async Task ModifyWrappingTransaction_should_return_true_if_modification_succeed()
        {
            var pendingTransaction = _mockCreator.GetWrappingTransaction(TransactionState.Pending);
            
            var savedTransaction = new WrappingTransactionDao(pendingTransaction);
            await _context.WrappingTransactions.AddAsync(savedTransaction);
            await _context.SaveChangesAsync();

            var completedTransaction = new WrappingTransactionDao(pendingTransaction);
            completedTransaction.EthereumBlockId = 12345;
            completedTransaction.NativeChainBlockId = 987654;
            completedTransaction.TransactionState = TransactionState.Complete;

            var isModify = await _transactionDataModifier.ModifyWrappingTransaction(completedTransaction);

            var retrievedTransaction =
                await _context.WrappingTransactions.FirstOrDefaultAsync(w =>
                    w.EthereumTransactionHash == savedTransaction.EthereumTransactionHash ||
                    w.NativeChainTransactionHash == savedTransaction.NativeChainTransactionHash);

            isModify.Should().Be(true);
            retrievedTransaction.EthereumBlockId.Should().Be(completedTransaction.EthereumBlockId);
            retrievedTransaction.Amount.Should().Be(completedTransaction.Amount);
            retrievedTransaction.ReceiverAddress.Should().Be(completedTransaction.ReceiverAddress);
            retrievedTransaction.SenderAddress.Should().Be(completedTransaction.SenderAddress);
            retrievedTransaction.TimeStamp.Should().Be(completedTransaction.TimeStamp);
            retrievedTransaction.User.Should().Be(completedTransaction.User);
            retrievedTransaction.TransactionState.Should().Be(completedTransaction.TransactionState);
        }
    }
}
