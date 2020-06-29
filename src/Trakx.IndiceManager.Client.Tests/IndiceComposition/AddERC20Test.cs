using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Syncfusion.Blazor;
using Trakx.Common.Models;
using Trakx.IndiceManager.ApiClient;
using Trakx.IndiceManager.Client.Pages.IndiceComposition;
using Trakx.Persistence.Tests;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Client.Tests.IndiceComposition
{
    public sealed class AddERC20Test : ComponentTest<AddERC20>
    {
        private readonly AddERC20.ComponentInput _inputComponent;
        private readonly MockCreator _mockCreator;
        private readonly IIndiceCreationClient _apiClient;

        public AddERC20Test(ITestOutputHelper output)
        {
            Services.AddSyncfusionBlazor();
            Services.AddSingleton(Substitute.For<ILogger<AddERC20>>());
            _apiClient = Substitute.For<IIndiceCreationClient>();
            Services.AddSingleton(_apiClient);
            Component = RenderComponent<AddERC20>();
            _mockCreator = new MockCreator(output);

            _inputComponent = Component.Instance.InputModel;
        }

        [Fact]
        public async Task Changing_address_in_textBox_should_change_ModelInput()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            await Dispatch(Component.Instance.AddressTextBox.ValueChanged, address);
            _inputComponent.Address.Should().Be(address);
        }

        [Fact]
        public async Task RetrievedComponent_should_be_not_null_after_calling_server()
        {
            var address = _mockCreator.GetRandomAddressEthereum();
            var componentModel = new ComponentDetailModel { Address = address };
            var response = new Response<ComponentDetailModel>((int)HttpStatusCode.Created, default, componentModel);
            _apiClient.GetComponentByAddressAsync(address).ReturnsForAnyArgs(response);

            await Component.Instance.GetComponent();
            Component.Instance.RetrievedComponent.Should().Be(componentModel);
        }

        [Fact]
        public async Task RetrievedComponent_should_be_null_if_calling_server_fail()
        {
            var exception = new ApiException("wrong way", 404, "You should not be here", null, null);

            var address = _mockCreator.GetRandomAddressEthereum();
            _apiClient.GetComponentByAddressAsync(address).ThrowsForAnyArgs(exception);

            await Component.Instance.GetComponent();
            Component.Instance.RetrievedComponent.Should().BeNull();
        }
    }
}
