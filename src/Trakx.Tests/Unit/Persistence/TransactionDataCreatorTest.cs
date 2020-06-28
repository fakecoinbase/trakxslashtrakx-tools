using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Persistence;
using Trakx.Persistence.DAO;
using Trakx.Tests.Data;
using Trakx.Tests.Unit.Models;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Persistence
{
    [Collection(nameof(EmptyDbContextCollection))]
    public sealed class TransactionDataCreatorTest
    {
        private readonly IndiceRepositoryContext _context;
        private readonly ITransactionDataCreator _transactionDataCreator;
        private readonly MockCreator _mockCreator;

        public TransactionDataCreatorTest(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _transactionDataCreator = new TransactionDataCreator(_context);
            _mockCreator = new MockCreator(output);
        }

        [Fact]
        public async Task SaveIndiceSupplyTransaction_should_return_true_if_addition_succeed()
        {
            var compositionToSave = new IndiceCompositionDao(new IndiceDefinitionDao(_mockCreator.GetRandomIndiceSymbol(), "SimplierTest", "nothing to say", 8, "EthereumAddress", DateTime.Now), 1, DateTime.Today, null);
            var transactionToSave = new IndiceSupplyTransactionDao(new DateTime(2015, 8, 10), compositionToSave, SupplyTransactionType.Redeem, 10m, "0x2317D87e46691ECc6203514A4c43fd806db281ff", "Aymeric", _mockCreator.GetRandomEthereumTransactionHash(), null);
            
            var isAdded = await _transactionDataCreator.SaveIndiceSupplyTransaction(transactionToSave);
            var retrievedTransaction = await
                _context.IndiceSupplyTransactions.FirstOrDefaultAsync(s =>
                    s.TransactionHash == transactionToSave.TransactionHash);

            isAdded.Should().BeTrue();
            retrievedTransaction.IndiceCompositionDao.Id.Should().Be(transactionToSave.IndiceCompositionDao.Id);
            retrievedTransaction.IndiceCompositionDao.IndiceDefinitionDao.Name.Should()
                .Be(transactionToSave.IndiceCompositionDao.IndiceDefinitionDao.Name);
            retrievedTransaction.Quantity.Should().Be(transactionToSave.Quantity);
        }

        [Fact]
        public async Task SaveWrappingTransaction_should_return_true_if_addition_succeed()
        {
            var wrappingTransaction = _mockCreator.GetWrappingTransaction(TransactionState.Pending);
            var transactionToSave = new WrappingTransactionDao(wrappingTransaction);

            var isAdded = await _transactionDataCreator.SaveWrappingTransaction(transactionToSave);
            var retrievedTransaction = await _context.WrappingTransactions.FirstOrDefaultAsync(w =>
                w.EthereumTransactionHash == transactionToSave.EthereumTransactionHash);

            isAdded.Should().BeTrue();
            retrievedTransaction.SenderAddress.Should().Be(transactionToSave.SenderAddress);
            retrievedTransaction.NativeChainTransactionHash.Should().Be(transactionToSave.NativeChainTransactionHash);
            retrievedTransaction.EthereumBlockId.Should().Be(transactionToSave.EthereumBlockId);
        }

    }
}
