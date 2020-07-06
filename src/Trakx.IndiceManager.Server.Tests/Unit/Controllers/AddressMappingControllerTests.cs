using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Core;
using Trakx.Common.Interfaces;
using Trakx.IndiceManager.Server.Controllers;
using Trakx.IndiceManager.Server.Managers;
using Trakx.Persistence.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Server.Tests.Unit.Controllers
{
    public class AddressMappingControllerTests
    {
        private readonly ICoinbaseInformationRetriever _coinbaseRetriever;
        private readonly AddressMappingController _controller;
        private readonly MockCreator _mockCreator;
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly IDepositorAddressRetriever _depositorAddressRetriever;

        public AddressMappingControllerTests(ITestOutputHelper output)
        {
            _coinbaseRetriever = Substitute.For<ICoinbaseInformationRetriever>();
            _coinbaseClient = Substitute.For<ICoinbaseClient>();
            _depositorAddressRetriever = Substitute.For<IDepositorAddressRetriever>();
            _mockCreator = new MockCreator(output);
            _controller = new AddressMappingController(_coinbaseRetriever, _coinbaseClient, _depositorAddressRetriever);
        }

        [Fact]
        public async Task GetTrakxAddress_should_return_address_when_symbol_is_found()
        {
            var symbol = _mockCreator.GetRandomCompositionSymbol();

            var expectedAddress = _mockCreator.GetRandomAddressEthereum();
            _coinbaseRetriever.GetTrakxAddressBySymbol(symbol).Returns(expectedAddress);

            var address = await _controller.GetTrakxAddress(symbol);

            var result = (string)((OkObjectResult)address.Result).Value;
            result.Should().Be(expectedAddress);

            await _coinbaseRetriever.ReceivedWithAnyArgs(1)
                .GetTrakxAddressBySymbol(default);
        }

        [Fact]
        public async Task GetTrakxAddress_should_not_return_address_for_null_or_empty_symbol()
        {
            var invalidSymbol = "";
            await _controller.GetTrakxAddress(invalidSymbol);

            await _coinbaseRetriever.DidNotReceiveWithAnyArgs()
                .GetTrakxAddressBySymbol(default);
        }

        [Fact]
        public async Task GetTrakxAddress_should_return_error_when_symbol_is_not_found()
        {
            var symbol = _mockCreator.GetRandomCompositionSymbol();
            await _controller.GetTrakxAddress(symbol);
            _coinbaseRetriever.GetTrakxAddressBySymbol(default)
                .ReturnsForAnyArgs((string)null);

            var address = await _controller.GetTrakxAddress(symbol);
            var result = (NotFoundObjectResult)address.Result;
            result.StatusCode.Should().Be(404);
            result.Value.Should().Be($"Sorry {symbol} doesn't have any corresponding address on trakx wallet.");
        }

        [Fact]
        public async Task GetAllSymbolAvailableOnCoinbase_should_send_a_list_of_symbols()
        {
            var symbolList = new List<string>
            {
                _mockCreator.GetRandomCompositionSymbol(),
                _mockCreator.GetRandomCompositionSymbol()
            };
            _coinbaseRetriever.GetAllCurrencySymbols().ReturnsForAnyArgs(symbolList);

            var result = await _controller.GetAllSymbolAvailableOnCoinbase();
            var returnedList = (List<string>)((OkObjectResult)result.Result).Value;

            returnedList.Count.Should().Be(2);
            returnedList[0].Should().Be(symbolList[0]);
            returnedList[0].Should().Be(symbolList[0]);
            returnedList[1].Should().Be(symbolList[1]);
        }

        [Fact]
        public async Task GetAllSymbolAvailableOnCoinbase_should_return_not_found_error_if_database_is_empty()
        {
            _coinbaseRetriever.GetAllCurrencySymbols().ReturnsForAnyArgs(new List<string>());

            var result = await _controller.GetAllSymbolAvailableOnCoinbase();
            ((NotFoundObjectResult)result.Result).Value.Should()
                .Be("Sorry, impossible to retrieve all currency symbols on Coinbase.");
        }

        [Theory]
        [InlineData("abc", null)]
        [InlineData("abc", "")]
        [InlineData("abc", " ")]
        [InlineData("abc", "98098hg")]
        [InlineData(null, "98098hgdsajdkajhkkuwaka87")]
        [InlineData("", "98098hgdsajdkajhkkuwaka87")]
        [InlineData(" ", "98098hgdsajdkajhkkuwaka87")]
        [InlineData("s", "98098hgdsajdkajhkkuwaka87")]
        public async Task RegisterUserAsAddressOwner_should_not_process_invalid_depositAddress(string currencySymbol, string address)
        {
            var user = Substitute.For<IUser>();
            var badAddress = new DepositAddressModel { CurrencySymbol = currencySymbol, Address = address };

            var result = await _controller.RegisterUserAsAddressOwner(badAddress, user, default).ConfigureAwait(false);
            var errorMessage = ((BadRequestObjectResult)result.Result).Value.ToString();
            errorMessage.Should().StartWith("Invalid deposit address, please try again.");

            await _depositorAddressRetriever.DidNotReceiveWithAnyArgs().GetDepositorAddressById(default);
            await _depositorAddressRetriever.DidNotReceiveWithAnyArgs().AssociateCandidateUser(default, default, default);
            await _coinbaseClient.DidNotReceiveWithAnyArgs().GetCurrencyAsync(default);
        }

        [Fact]
        public async Task RegisterUserAsAddressOwner_should_failed_on_previously_verified_address()
        {
            var user = Substitute.For<IUser>();
            var verifiedAddress = new DepositAddressModel { CurrencySymbol = "abc", Address = "alreadyverified" };
            var depositAddress = verifiedAddress.ToDepositAddress();
            depositAddress.IsVerified = true;

            _depositorAddressRetriever.GetDepositorAddressById(verifiedAddress.ToDepositAddress().Id)
                .Returns(depositAddress);

            var result = await _controller.RegisterUserAsAddressOwner(verifiedAddress, user, default).ConfigureAwait(false);
            var errorMessage = ((BadRequestObjectResult)result.Result).Value.ToString();
            errorMessage.Should().Be("This address has already been verified.");

            await _depositorAddressRetriever.DidNotReceiveWithAnyArgs().AssociateCandidateUser(default, default, default);
            await _coinbaseClient.DidNotReceiveWithAnyArgs().GetCurrencyAsync(default);
        }


        [Fact]
        public async Task RegisterUserAsAddressOwner_should_find_decimals_and_associate_user_with_address()
        {
            var user = Substitute.For<IUser>();
            var verifiedAddress = new DepositAddressModel { CurrencySymbol = "abc", Address = "neververified" };
            var depositAddress = verifiedAddress.ToDepositAddress();
            depositAddress.IsVerified = false;

            var addressAfterAssociationWithCandidate = new DepositorAddress(depositAddress.Address,
                depositAddress.CurrencySymbol,
                verificationAmount: 12.34m, user: user);

            _depositorAddressRetriever.GetDepositorAddressById(verifiedAddress.ToDepositAddress().Id)
                .Returns(depositAddress, addressAfterAssociationWithCandidate);

            _depositorAddressRetriever.AssociateCandidateUser(depositAddress, user, 2)
                .Returns(true);

            _coinbaseClient.GetCurrencyAsync(depositAddress.CurrencySymbol).Returns(new Currency { Decimals = 2 });

            var result = await _controller.RegisterUserAsAddressOwner(verifiedAddress, user, default).ConfigureAwait(false);
            var acceptedResult = (IDepositorAddress)((AcceptedResult)result.Result).Value;

            acceptedResult.User.Should().Be(user);
            acceptedResult.VerificationAmount.Should().Be(12.34m);
        }
    }
}
