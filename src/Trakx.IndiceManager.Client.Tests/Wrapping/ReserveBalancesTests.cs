using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Syncfusion.Blazor;
using Trakx.Common.Models;
using Trakx.IndiceManager.ApiClient;
using Trakx.IndiceManager.Client.Pages.Wrapping;
using Trakx.IndiceManager.Client.Shared;
using Trakx.Persistence.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Client.Tests.Wrapping
{
    public sealed class ReserveBalancesTests : ComponentTest<ReserveBalances>
    {
        private readonly MockCreator _mockCreator;
        private readonly ReserveBalances _componentInstance;
        private readonly List<AccountBalanceModel> _expectedBalances;
        private readonly IWrappingClient _wrappingClient;
        private readonly IToaster _toaster;

        public ReserveBalancesTests(ITestOutputHelper output)
        {
            Services.AddSyncfusionBlazor();

            _toaster = Substitute.For<IToaster>();
            Services.AddSingleton(_toaster);

            _wrappingClient = Substitute.For<IWrappingClient>();
            Services.AddSingleton(_wrappingClient);

            _mockCreator = new MockCreator(output);

            _expectedBalances = SetupMockWalletsResponse();

            Component = RenderComponent<ReserveBalances>();
            _componentInstance = Component.Instance;
        }

        private List<AccountBalanceModel> SetupMockWalletsResponse()
        {
            var wallets = new[] { 
                _mockCreator.GetRandomAccountBalanceModel(), 
                _mockCreator.GetRandomAccountBalanceModel()
            }.ToList();
            _wrappingClient.GetTrakxBalancesAsync().ReturnsForAnyArgs(
                new Response<List<AccountBalanceModel>>(StatusCodes.Status200OK, default, wallets));
            return wallets;
        }
        

        [Fact]
        public async Task AccountBalances_should_be_loaded_on_initialization()
        {
            await _wrappingClient.ReceivedWithAnyArgs(1).GetTrakxBalancesAsync();
            _componentInstance.AccountBalances.Should().BeEquivalentTo(_expectedBalances);
        }

        [Fact]
        public async Task Wallets_should_be_empty_when_server_call_failed()
        {
            var apiException = new ApiException("wrong route", 404, "page not found", null, null);
            _wrappingClient.GetTrakxBalancesAsync().ThrowsForAnyArgs(apiException);

            await Dispatch(() => Component = RenderComponent<ReserveBalances>());

            Component.Instance.AccountBalances.Should().BeEmpty();
            await _toaster.Received(1).ShowError(Arg.Any<string>());
        }

        [Fact]
        public void DataGrid_should_show_AccountBalances()
        {
            Component = RenderComponent<ReserveBalances>();
            _componentInstance.DataGrid.DataSource.Should().BeEquivalentTo(_componentInstance.AccountBalances);
        }
    }
}
