using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Common.Models;
using Trakx.IndiceManager.Server.Managers;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Server.Tests.Unit.Managers
{
    public class IndiceSupplyServiceTest
    {
        private readonly IIndiceSupplyService _indiceSupplyService;
        private readonly ITransactionDataProvider _dataProvider;
        private readonly ITransactionDataCreator _dataCreator;
        private readonly IndiceSupplyTransactionModel _transactionModel;

        public IndiceSupplyServiceTest(ITestOutputHelper output)
        {
            _dataProvider = Substitute.For<ITransactionDataProvider>();
            _dataCreator = Substitute.For<ITransactionDataCreator>();
            _indiceSupplyService= new IndiceSupplyService(_dataCreator,_dataProvider);
            var mockCreator = new MockCreator(output);
            _transactionModel= new IndiceSupplyTransactionModel()
            {
                CreationTimestamp = DateTime.MaxValue,
                IndiceComposition = new IndiceCompositionModel(mockCreator.GetIndiceComposition(0)),
                IndiceQuantity = 2m,
                SenderAddress = mockCreator.GetRandomAddressEthereum(),
                TransactionType = SupplyTransactionType.Issue,
                User = "jerem"
            };
            
        }

        [Fact]
        public async Task TryToSaveTransaction_should_return_true_if_addition_succeeded()
        {
             _dataCreator.SaveIndiceSupplyTransaction(default).ReturnsForAnyArgs(true);
             var isAdded = await _indiceSupplyService.TryToSaveTransaction(_transactionModel);

             isAdded.Should().BeTrue();
        }

        [Fact]
        public async Task TryToSaveTransaction_should_return_false_if_addition_failed()
        {
            _dataCreator.SaveIndiceSupplyTransaction(default).ReturnsForAnyArgs(false);
            var isAdded = await _indiceSupplyService.TryToSaveTransaction(_transactionModel);

            isAdded.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllTransactionByUser_should_return_empty_list_if_user_didnt_make_any_transaction()
        {
            _dataProvider.GetAllIndiceSupplyTransactionsByUser("aymeric").Returns(new List<IIndiceSupplyTransaction>());
            var retrievedTransactions = await _indiceSupplyService.GetAllTransactionByUser("aymeric");
            retrievedTransactions.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetAllTransactionByUser_should_return_list_of_transaction()
        {
            _dataProvider.GetAllIndiceSupplyTransactionsByUser("jerem").Returns(new List<IIndiceSupplyTransaction>
            {
                new IndiceSupplyTransaction(DateTime.Now, null, SupplyTransactionType.Issue, 2m, null, "jerem", null,
                    null)
            });

            var retrievedTransactions = await _indiceSupplyService.GetAllTransactionByUser("jerem");
            retrievedTransactions.Count.Should().Be(1);
        }
    }
}
