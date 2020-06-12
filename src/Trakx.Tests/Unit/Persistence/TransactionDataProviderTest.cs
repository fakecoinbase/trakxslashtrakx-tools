using System;
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
    public sealed class TransactionDataProviderTest
    {
        private readonly IndiceRepositoryContext _context;
        private readonly ITransactionDataProvider _transactionDataProvider;
        private readonly MockCreator _mockCreator;

        public TransactionDataProviderTest(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _transactionDataProvider = new TransactionDataProvider(_context);
            _mockCreator = new MockCreator(output);
        }

        [Fact]
        public async Task GetWrappingTransactionByNativeTransactionHash_should_return_null_when_transaction_not_in_database()
        {
            var result =
                await _transactionDataProvider.GetWrappingTransactionByNativeTransactionHash(_mockCreator.GetRandomString(50));

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetWrappingTransactionByNativeTransactionHash_should_return_the_transaction()
        {
            var transaction = await AddNewWrappingTransactionInDatabase();
            
            var result = await _transactionDataProvider
                .GetWrappingTransactionByNativeTransactionHash(transaction.NativeChainTransactionHash);

            RetrievedWrappingTransactionShouldBeTheGoodOne(result, transaction);
        }

        [Fact]
        public async Task GetWrappingTransactionByEthereumTransactionHash_should_return_null_if_transaction_not_in_database()
        {
            var result =
                await _transactionDataProvider.GetWrappingTransactionByNativeTransactionHash(_mockCreator.GetRandomString(50));

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetWrappingTransactionByEthereumTransactionHash_should_return_the_transaction()
        {
            var transaction = await AddNewWrappingTransactionInDatabase();

            var result =
                await _transactionDataProvider.GetWrappingTransactionByEthereumTransactionHash(transaction.EthereumTransactionHash);

            RetrievedWrappingTransactionShouldBeTheGoodOne(result, transaction);
        }

        [Fact]
        public async Task GetAllWrappingTransactionsByUser_should_return_empty_list_if_user_didnt_make_transaction()
        {
            var result = await _transactionDataProvider.GetAllWrappingTransactionsByUser("userTest12/05/2020;09h57");
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllIndiceSupplyTransactionsByUser_should_return_empty_list_if_user_didnt_make_transaction()
        {
            var result = await _transactionDataProvider.GetAllWrappingTransactionsByUser("userTest12/05/2020;09h58;125seconds");
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWrappingTransactionsByUser_should_return_list_of_wrappingTransaction()
        {
            var transaction = await AddNewWrappingTransactionInDatabase().ConfigureAwait(false);

            var result = await _transactionDataProvider.GetAllWrappingTransactionsByUser(transaction.User);

            result.Count.Should().NotBe(0);
            RetrievedWrappingTransactionShouldBeTheGoodOne(result[0], transaction);
        }

        [Fact]
        public async Task GetAllIndiceSupplyTransactionsByUser_should_return_list_of_indiceSupplyTransaction()
        {
            var composition = new IndiceCompositionDao(new IndiceDefinitionDao(_mockCreator.GetRandomIndiceSymbol(), "SimplierTest", "nothing to say", 8, "EthereumAddress", DateTime.Now), 1, DateTime.Today, null, null);
            var userName = "Aymeric"+_mockCreator.GetRandomString(10);
            var transaction = new IndiceSupplyTransactionDao(new DateTime(2015, 8, 10), composition, SupplyTransactionType.Redeem, 10m, "0x2317D87e46691ECc6203514A4c43fd806db281ff", userName, null, null);
            await _context.IndiceSupplyTransactions.AddRangeAsync(transaction);
            await _context.SaveChangesAsync();

            var result = await _transactionDataProvider.GetAllIndiceSupplyTransactionsByUser(userName);

            result.Count.Should().Be(1);
            result[0].TransactionType.Should().Be(transaction.TransactionType);
            result[0].CreationTimestamp.Should().Be(transaction.CreationTimestamp);
            result[0].SenderAddress.Should().Be(transaction.SenderAddress);
        }

        [Fact]
        public async Task GetLastWrappingTransactionDatetime_should_have_the_bigger_timestamp()
        {
            await AddNewWrappingTransactionInDatabase(DateTime.MaxValue);
            await AddNewWrappingTransactionInDatabase(new DateTime(2021, 10, 10));
            

            var timestamp = await _transactionDataProvider.GetLastWrappingTransactionDatetime();
            timestamp.Should().Be(DateTime.MaxValue);
        }

        public async Task<WrappingTransactionDao> AddNewWrappingTransactionInDatabase(DateTime? transactionDateTime=default)
        {
            var transaction = _mockCreator.GetWrappingTransaction(TransactionState.Pending);
            var transactionDao = new WrappingTransactionDao(transaction);
            transactionDao.TimeStamp = transactionDateTime ?? transactionDao.TimeStamp;
            await _context.WrappingTransactions.AddAsync(transactionDao);
            await _context.SaveChangesAsync();
            return transactionDao;
        }

        private static void RetrievedWrappingTransactionShouldBeTheGoodOne(IWrappingTransaction retrieved,
            WrappingTransactionDao expected)
        {
            retrieved.User.Should().Be(expected.User);
            retrieved.SenderAddress.Should().Be(expected.SenderAddress);
            retrieved.Amount.Should().Be(expected.Amount);
            retrieved.TimeStamp.Should().Be(expected.TimeStamp);
            retrieved.ReceiverAddress.Should().Be(expected.ReceiverAddress);
            retrieved.NativeChainTransactionHash.Should().Be(expected.NativeChainTransactionHash);
        }

    }
}
