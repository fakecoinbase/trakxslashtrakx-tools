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
        private readonly IUserAddressProvider _userAddressProvider;
        private readonly IUserBalanceUpdater _balanceUpdater;
        private readonly MockDaoCreator _daoCreator;

        public BalanceUpdaterTest(ITestOutputHelper output)
        {
            _userAddressProvider = Substitute.For<IUserAddressProvider>();
            var serviceScopeFactory = PrepareScopeResolution();
            _balanceUpdater = new UserBalanceUpdater(serviceScopeFactory);
            _daoCreator = new MockDaoCreator(output);
        }
        private IServiceScopeFactory PrepareScopeResolution()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService<IUserAddressProvider>().Returns(_userAddressProvider);
            var serviceScope = Substitute.For<IServiceScope>();
            serviceScope.ServiceProvider.Returns(serviceProvider);
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            serviceScopeFactory.CreateScope().Returns(serviceScope);
            return serviceScopeFactory;
        }


        [Fact]
        public void OnNext_should_creates_new_temporaryMapping_if_transactionSender_is_unknown()
        {
            var transaction = new ProcessedTransaction(3, new Transaction());
            _userAddressProvider.TryToGetUserAddressByAddress(default).ReturnsForAnyArgs((IUserAddress)null);

            _balanceUpdater.OnNext(transaction);
            _userAddressProvider.ReceivedWithAnyArgs(1).AddNewMapping(default);
            _userAddressProvider.DidNotReceiveWithAnyArgs().UpdateUserBalance(default);
            _userAddressProvider.DidNotReceiveWithAnyArgs().ValidateMappingAddress(default);
        }

        [Fact]
        public void TryUpdateUserBalance_should_validateMapping_when_sentAmount_is_verificationAmount()
        {
            var retrievedUser = _daoCreator.GetRandomUserAddressDao(-50);
            retrievedUser.IsVerified.Should().BeFalse();
            var processedTransaction = new ProcessedTransaction(1, new Transaction { Amount = 5 });
            _userAddressProvider.UpdateUserBalance(default).ReturnsForAnyArgs(true);
            _userAddressProvider.ValidateMappingAddress(default).ReturnsForAnyArgs(true);

            _balanceUpdater.TryUpdateUserBalance(retrievedUser, processedTransaction);
            _userAddressProvider.ReceivedWithAnyArgs(1).ValidateMappingAddress(default);
            _userAddressProvider.DidNotReceiveWithAnyArgs().UpdateUserBalance(default);
        }

        [Fact]
        public void TryUpdateUserBalance_should_TryToUpdate_UserBalance()
        {
            var retrievedUser = _daoCreator.GetRandomUserAddressDao();
            retrievedUser.IsVerified = true;
            var transaction = new ProcessedTransaction(10, new Transaction { Amount = 10 });
            _balanceUpdater.TryUpdateUserBalance(retrievedUser, transaction);
            _userAddressProvider.ReceivedWithAnyArgs(1).UpdateUserBalance(default);
            _userAddressProvider.DidNotReceiveWithAnyArgs().ValidateMappingAddress(default);
        }

    }
}
