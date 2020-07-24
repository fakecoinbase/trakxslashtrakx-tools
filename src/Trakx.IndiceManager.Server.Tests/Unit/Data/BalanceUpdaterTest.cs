using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Interfaces;
using Trakx.IndiceManager.Server.Data;
using Trakx.Persistence.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Server.Tests.Unit.Data
{
    public class BalanceUpdaterTest
    {
        private readonly IExternalAddressRetriever _externalAddressRetriever;
        private readonly IBalanceUpdater _balanceUpdater;
        private readonly MockDaoCreator _daoCreator;

        public BalanceUpdaterTest(ITestOutputHelper output)
        {
            _externalAddressRetriever = Substitute.For<IExternalAddressRetriever>();
            var serviceScopeFactory = PrepareScopeResolution();
            _balanceUpdater = new BalanceUpdater(serviceScopeFactory);
            _daoCreator = new MockDaoCreator(output);
        }
        private IServiceScopeFactory PrepareScopeResolution()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService<IExternalAddressRetriever>().Returns(_externalAddressRetriever);
            var serviceScope = Substitute.For<IServiceScope>();
            serviceScope.ServiceProvider.Returns(serviceProvider);
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            serviceScopeFactory.CreateScope().Returns(serviceScope);
            return serviceScopeFactory;
        }

        private CoinbaseTransaction GetRandomCoinbaseTransaction()
        {
            var transaction = new CoinbaseTransaction
            {
                Currency = _daoCreator.GetRandomString(3),
                Source = _daoCreator.GetRandomAddressEthereum(),
                Amount = _daoCreator.GetRandomPrice()
            };
            return transaction;
        }


        [Fact]
        public void OnNext_should_creates_new_temporaryMapping_if_transactionSender_is_unknown()
        {
            var transaction = GetRandomCoinbaseTransaction();
            _externalAddressRetriever.GetExternalAddressById(default).ReturnsForAnyArgs((IExternalAddress)null);

            _balanceUpdater.OnNext(transaction);
            _externalAddressRetriever.ReceivedWithAnyArgs(1)
                .AddNewAddress(Arg.Is<IExternalAddress>(a => a.Address == transaction.Source
                                                              && a.Balance == transaction.Amount));
            _externalAddressRetriever.DidNotReceiveWithAnyArgs().UpdateExternalAddress(default);
        }

        [Fact]
        public void OnNext_should_verify_address_when_sentAmount_is_verificationAmount()
        {
            var transaction = GetRandomCoinbaseTransaction();
            var retrievedAddress = _daoCreator.GetRandomExternalAddressDao(transaction.Amount);
            retrievedAddress.IsVerified.Should().BeFalse();

            _balanceUpdater.OnNext(transaction);

            _externalAddressRetriever.DidNotReceiveWithAnyArgs().AddNewAddress(default);
            _externalAddressRetriever.ReceivedWithAnyArgs(1).UpdateExternalAddress(
                Arg.Is<IExternalAddress>(a => a.Address == transaction.Source && a.IsVerified));
        }

        [Fact]
        public void OnNext_should_TryToUpdate_UserBalance()
        {
            var address = _daoCreator.GetRandomExternalAddressDao(isVerified: true);
            var transaction = GetRandomCoinbaseTransaction();
            _balanceUpdater.OnNext(transaction);
            _externalAddressRetriever.Received(1)
                .UpdateExternalAddress(Arg.Is<IExternalAddress>(t => t.Balance == address.Balance + transaction.Amount));
        }

    }
}
