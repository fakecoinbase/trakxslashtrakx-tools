using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Common.Interfaces;
using Trakx.IndiceManager.Server.Data;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Server.Tests.Unit.Data
{
    public class BalanceUpdaterTest
    {
        private readonly IDepositorAddressRetriever _depositorAddressRetriever;
        private readonly IBalanceUpdater _balanceUpdater;
        private readonly MockDaoCreator _daoCreator;

        public BalanceUpdaterTest(ITestOutputHelper output)
        {
            _depositorAddressRetriever = Substitute.For<IDepositorAddressRetriever>();
            var serviceScopeFactory = PrepareScopeResolution();
            _balanceUpdater = new BalanceUpdater(serviceScopeFactory);
            _daoCreator = new MockDaoCreator(output);
        }
        private IServiceScopeFactory PrepareScopeResolution()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService<IDepositorAddressRetriever>().Returns(_depositorAddressRetriever);
            var serviceScope = Substitute.For<IServiceScope>();
            serviceScope.ServiceProvider.Returns(serviceProvider);
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            serviceScopeFactory.CreateScope().Returns(serviceScope);
            return serviceScopeFactory;
        }

        private CoinbaseTransaction GetRandomCoinbaseTransaction()
        {
            var transaction = new CoinbaseTransaction(
                    new CoinbaseRawTransaction
                    {
                        Currency = _daoCreator.GetRandomString(3), 
                        Source = _daoCreator.GetRandomAddressEthereum(),
                        UnscaledAmount = _daoCreator.GetRandomUnscaledAmount(),
                    }, _daoCreator.GetRandomNaturalUnit());
            return transaction;
        }


        [Fact]
        public void OnNext_should_creates_new_temporaryMapping_if_transactionSender_is_unknown()
        {
            var transaction = GetRandomCoinbaseTransaction();
            _depositorAddressRetriever.GetDepositorAddressById(default).ReturnsForAnyArgs((IDepositorAddress)null);

            _balanceUpdater.OnNext(transaction);
            _depositorAddressRetriever.ReceivedWithAnyArgs(1)
                .AddNewAddress(Arg.Is<IDepositorAddress>(a => a.Address == transaction.Source 
                                                              && a.Balance == transaction.ScaledAmount));
            _depositorAddressRetriever.DidNotReceiveWithAnyArgs().UpdateDepositorAddress(default);
        }

        [Fact]
        public void OnNext_should_verify_address_when_sentAmount_is_verificationAmount()
        {
            var transaction = GetRandomCoinbaseTransaction();
            var retrievedAddress = _daoCreator.GetRandomDepositorAddressDao(transaction.ScaledAmount);
            retrievedAddress.IsVerified.Should().BeFalse();

            _balanceUpdater.OnNext(transaction);

            _depositorAddressRetriever.DidNotReceiveWithAnyArgs().AddNewAddress(default);
            _depositorAddressRetriever.ReceivedWithAnyArgs(1).UpdateDepositorAddress(
                Arg.Is<IDepositorAddress>(a => a.Address == transaction.Source && a.IsVerified));
        }

        [Fact]
        public void OnNext_should_TryToUpdate_UserBalance()
        {
            var address = _daoCreator.GetRandomDepositorAddressDao(isVerified: true);
            var transaction = GetRandomCoinbaseTransaction();
            _balanceUpdater.OnNext(transaction);
            _depositorAddressRetriever.Received(1)
                .UpdateDepositorAddress(Arg.Is<IDepositorAddress>(t => t.Balance == address.Balance + transaction.ScaledAmount));
        }

    }
}
