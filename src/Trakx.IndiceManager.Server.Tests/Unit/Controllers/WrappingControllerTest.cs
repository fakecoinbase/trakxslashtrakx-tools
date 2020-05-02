using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Transaction;
using Trakx.Common.Sources.Coinbase;
using Trakx.IndiceManager.Server.Controllers;
using Trakx.IndiceManager.Server.Managers;
using Trakx.IndiceManager.Server.Models;
using Trakx.Tests.Data;
using Xunit;

namespace Trakx.IndiceManager.Server.Tests.Unit.Controllers
{
    public class WrappingControllerTest
    {
        private readonly IWrappingService _wrappingService;
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly WrappingController _controller;
        private readonly MockCreator _mockCreator;

        public WrappingControllerTest()
        {
            _wrappingService = Substitute.For<IWrappingService>();
            _coinbaseClient = Substitute.For<ICoinbaseClient>();
            _controller = new WrappingController(_wrappingService,_coinbaseClient);
            _mockCreator= new MockCreator();
        }

        [Fact]
        public async Task GetTrakxAddressFromSymbol_should_not_send_200status_if_symbol_not_supported_by_coinbase()
        {
            var fakeSymbol = "FAKESYMBOL";
            _coinbaseClient.CustodiedCoins.Returns(new List<string>{"btc","eth"});

            await _controller.GetTrakxAddressFromSymbol(fakeSymbol);
            await _wrappingService.DidNotReceive().RetrieveAddressFromSymbol(fakeSymbol);
        }

        [Fact]
        public async Task GetTrakxAddressFromSymbol_should_send_TrakxAddress_associated_with_symbol()
        {
            var symbol = "btc";
            var accountAddress = "0x734Ac651Dd95a339c633cdEd410228515F97fAfF";
            _coinbaseClient.CustodiedCoins.Returns(new List<string> {"btc", "eth", "ltc"});
            _wrappingService.RetrieveAddressFromSymbol(symbol).Returns(accountAddress);

            var result = await _controller.GetTrakxAddressFromSymbol(symbol);
            ((OkObjectResult) result.Result).Value.Should().Be(accountAddress);
        }

        [Fact]
        public async Task GetTrakxAddressFromSymbol_should_send_badrequest_if_error_occurs_when_connecting_to_coinbase()
        {
            var symbol = "btc";
            
            _coinbaseClient.CustodiedCoins.Returns(new List<string> { "btc", "eth", "ltc" });
            _wrappingService.RetrieveAddressFromSymbol(symbol).Returns((string)null);

            var result = await _controller.GetTrakxAddressFromSymbol(symbol);
            ((BadRequestObjectResult)result.Result).Value.Should().Be("Sorry we can't find the address, please try again.");
        }

        [Fact]
        public async Task WrapTokens_should_not_send_wrap_tokens_if_user_didnt_make_any_transaction()
        {
            var transaction = new WrappingTransactionModel();
            _wrappingService.TryToFindTransaction(transaction).Returns((string) null);

            await _controller.WrapTokens(transaction);
            await _wrappingService.DidNotReceiveWithAnyArgs().InitiateWrapping(default);
        }

        [Fact]
        public async Task WrapTokens_should_send_wrap_tokens_if_user_make_first_transaction()
        {
            var transaction = new WrappingTransactionModel();
            string transactionHash = "0xc3f5d7e6edd714cfb6ee0091765ef0ebdf991223e45b53fe2b5a26f870997a3f";
            _wrappingService.TryToFindTransaction(transaction).Returns(transactionHash);

            var result = await _controller.WrapTokens(transaction);
            ((OkObjectResult) result.Result).Value.Should().Be("Transaction succeed.");
        }

        [Fact]
        public async Task GetAllTransactionByUser_should_not_send_transactions_if_user_didnt_make_any()
        {
            string user = "TrakxTest :)";
            _wrappingService.GetTransactionByUser(user).Returns((List<IWrappingTransaction>)null);

            var result = await _controller.GetAllTransactionByUser(user);
            ((BadRequestObjectResult) result.Result).Value.Should().Be("This User hasn't made any transactions.");
        }

        [Fact]
        public async Task GetAllTransactionByUser_should_not_send_transactions()
        {
            string user = "TrakxTest :)";
            List<IWrappingTransaction> transactions= new List<IWrappingTransaction>
            {
                _mockCreator.GetWrappingTransaction(),
                _mockCreator.GetWrappingTransaction()
            };
            _wrappingService.GetTransactionByUser(user).Returns(transactions);

            var result = await _controller.GetAllTransactionByUser(user);
            var finalResult = (List<WrappingTransactionModel>)((OkObjectResult)result.Result).Value;
            finalResult[0].Date.Should().Be(transactions[0].TimeStamp);
            finalResult[0].EthereumBlockId.Should().Be(transactions[0].EthereumBlockId);
            finalResult[1].FromCurrency.Should().Be(transactions[1].FromCurrency);
            finalResult[1].SenderAddress.Should().Be(transactions[1].SenderAddress);
        }

        [Fact]
        public async Task GetTrakxBalance_should_not_send_balance_if_error_occurs_at_coinbase()
        {
            _wrappingService.GetBalances().Returns((List<AccountBalanceModel>) null);
            
            var result = await _controller.GetTrakxBalance();
            ((BadRequestObjectResult) result.Result).Value.Should().Be("An error occurred, please try again.");
        }

        [Fact]
        public async Task GetTrakxBalance_should_send_TrakxBalance()
        {
            var balance = new List<AccountBalanceModel>
            {
                new AccountBalanceModel("BTC", 0.2m),
                new AccountBalanceModel("ETH", 25.3m),
                new AccountBalanceModel("WBTC", 62.2m)
            };
            _wrappingService.GetBalances().Returns(balance);
            var result = await _controller.GetTrakxBalance();
            var finalResult = (List<AccountBalanceModel>) ((OkObjectResult) result.Result).Value;
            finalResult[0].Should().Be(balance[0]);
            finalResult[1].Should().Be(balance[1]);
        }
    }
}
