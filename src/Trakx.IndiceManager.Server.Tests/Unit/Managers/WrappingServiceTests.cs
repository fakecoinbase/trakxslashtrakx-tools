using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NSubstitute;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.IndiceManager.Server.Data;
using Trakx.IndiceManager.Server.Managers;
using Trakx.Persistence.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Server.Tests.Unit.Managers
{
    public class WrappingServiceTests
    {
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly WrappingService _wrappingService;
        private readonly MockCreator _mockCreator;

        public WrappingServiceTests(ITestOutputHelper output)
        {
            _mockCreator = new MockCreator(output);
            _coinbaseClient = Substitute.For<ICoinbaseClient>();
            _wrappingService = new WrappingService(_coinbaseClient, output.ToLogger<WrappingService>());
        }

        [Fact]
        public async Task GetTrakxBalances_should_retrieve_wallet_currency_details_from_cache()
        {
            var wallets = Enumerable.Range(0, 3).Select(i => _mockCreator.GetRandomWallet()).ToList();
            _coinbaseClient.GetWallets().ReturnsForAnyArgs(wallets.ToAsyncEnumerable());

            var balances = await _wrappingService.GetTrakxBalances().ToListAsync();

            _coinbaseClient.Received(1).GetWallets();
            
            foreach (var wallet in wallets)
            {
                var balance = balances.Single(b => b.Symbol == wallet.CurrencySymbol);
                balance.Address.Should().Be(wallet.ColdAddress);
                balance.Balance.Should().Be(wallet.Balance);
                balance.NativeBalance.Should().Be(wallet.UnscaledBalance);
                balance.Name.Should().Be(wallet.Name);
            }
        }
    }
}