using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Transaction;
using Trakx.IndiceManager.Server.Controllers;
using Trakx.IndiceManager.Server.Managers;
using Trakx.IndiceManager.Server.Models;
using Trakx.Tests.Data;
using Xunit;

namespace Trakx.IndiceManager.Server.Tests.Unit.Controllers
{
    public class IndiceSupplyControllerTest
    {
        private readonly IndiceSupplyController _controller;
        private readonly IIndiceSupplyService _indiceSupplyService;
        private readonly IndiceSupplyTransactionModel _transaction;
        private readonly MockCreator _mock = new MockCreator();

        public IndiceSupplyControllerTest()
        {
            _indiceSupplyService = Substitute.For<IIndiceSupplyService>();
            _controller=new IndiceSupplyController(_indiceSupplyService);
            _transaction= new IndiceSupplyTransactionModel
            {
                CreationTimestamp = DateTime.MaxValue,
                IndiceComposition = new IndiceCompositionModel(_mock.GetIndiceComposition(3)),
                IndiceQuantity = 4.00m,
                TransactionType = SupplyTransactionType.Redeem,
                User = "Fred",
                SenderAddress = "0xabcd123456"
            };
        }

        [Fact]
        public async Task SaveTransaction_should_return_error_when_user_not_defined()
        {
            _transaction.User = null;

            var result = await _controller.SaveTransaction(_transaction);
            ((BadRequestObjectResult) result.Result).Value.Should()
                .Be("There is no user attached to this transaction. Please try again.");
        }

        [Fact]
        public async Task SaveTransaction_should_return_error_when_some_fields_are_uninitiated()
        {
            _transaction.IndiceComposition = null;

            var result = await _controller.SaveTransaction(_transaction);
            ((BadRequestObjectResult) result.Result).Value.Should()
                .Be("Some mandatory parameters are missing, please try again.");
        }

        [Fact]
        public async Task SaveTransaction_should_return_error_if_something_go_wrong_with_the_database()
        {
            _indiceSupplyService.TryToSaveTransaction(_transaction).Returns(false);

            var result = await _controller.SaveTransaction(_transaction);
            ((BadRequestObjectResult)result.Result).Value.Should()
                .Be("The addition in the database failed. Please try again.");
        }

        [Fact]
        public async Task SaveTransaction_should_return_statusCode201_if_addition_in_database_succeed()
        {
            _indiceSupplyService.TryToSaveTransaction(_transaction).Returns(true);

            var result = await _controller.SaveTransaction(_transaction);
            ((CreatedAtActionResult) result.Result).ActionName.Should()
                .Be("The transaction has been added to the database");
            ((CreatedAtActionResult)result.Result).StatusCode.Should()
                .Be(201);
            ((CreatedAtActionResult)result.Result).Value.Should()
                .Be(_transaction);
        }

        [Fact]
        public async Task RetrieveTransactions_should_return_error_when_user_not_defined()
        {
            var result = await _controller.RetrieveTransactions(null);

            ((BadRequestObjectResult) result.Result).Value.Should()
                .Be("Can't retrieve all of the transaction because the user is undefined.");
        }

        [Fact]
        public async Task RetrieveTransactions_should_return_error_user_didnt_make_any_transaction()
        {
            _indiceSupplyService.GetAllTransactionByUser("name")
                .ReturnsForAnyArgs(new List<IIndiceSupplyTransaction>());

            var result = await _controller.RetrieveTransactions("name");

            ((BadRequestObjectResult)result.Result).Value.Should()
                .Be("The user didn't make any transaction.");
        }

        [Fact]
        public async Task RetrieveTransactions_should_return_list_of_IndiceSupplyTransactionModel()
        {
            var transactions= new List<IIndiceSupplyTransaction>
            {
                new IndiceSupplyTransaction(DateTime.Today, _mock.GetIndiceComposition(2),SupplyTransactionType.Redeem,0.3m,"senderAddress","fred",null,null)
            };

            _indiceSupplyService.GetAllTransactionByUser("fred")
                .ReturnsForAnyArgs(transactions);

            var result = await _controller.RetrieveTransactions("fred");

            var finalResult=(List<IndiceSupplyTransactionModel>) ((OkObjectResult) result.Result).Value;
            finalResult[0].User.Should().Be(transactions[0].User);
            finalResult[0].SenderAddress.Should().Be(transactions[0].SenderAddress);
            finalResult[0].CreationTimestamp.Should().Be(transactions[0].CreationTimestamp);
            finalResult[0].TransactionType.Should().Be(transactions[0].TransactionType);
            finalResult[0].IndiceQuantity.Should().Be(transactions[0].Quantity);
        }
    }
}
