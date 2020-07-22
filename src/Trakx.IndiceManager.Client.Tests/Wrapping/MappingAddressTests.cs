using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.IndiceManager.Client.Pages.Wrapping;
using Xunit;
using Xunit.Abstractions;
using Syncfusion.Blazor;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Trakx.IndiceManager.ApiClient;

namespace Trakx.IndiceManager.Client.Tests.Wrapping
{
    public sealed class MappingAddressTests : ComponentTest<AddressMapping>
    {
        private readonly MockCreator _mockCreator;
        private readonly IIndiceCreationClient _apiClient;
        private readonly IAddressMapping _addressMapping;
        private readonly AddressMapping _componentInstance;

        public MappingAddressTests(ITestOutputHelper output)
        {
            _mockCreator = new MockCreator(output);
            Services.AddSyncfusionBlazor();
            _apiClient = Substitute.For<IIndiceCreationClient>();
            SetMockApiClient();
            _addressMapping = Substitute.For<IAddressMapping>();
            Services.AddSingleton(_addressMapping);
            Component = RenderComponent<AddressMapping>();
            _componentInstance = Component.Instance;

        }

        public void SetMockApiClient()
        {
            var quantity = _mockCreator.GetComponentQuantity();
            var componentDetail = new ComponentDetailModel
            {
                Quantity = quantity.Quantity,
                Address = quantity.ComponentDefinition.Address,
                CoinGeckoId = quantity.ComponentDefinition.CoinGeckoId,
                Decimals = quantity.ComponentDefinition.Decimals,
                Name = quantity.ComponentDefinition.Name,
                Symbol = quantity.ComponentDefinition.Symbol,
            };
            var componentCollection = new List<ComponentDetailModel>();
            componentCollection.Add(componentDetail);
            var response = new Response<List<ComponentDetailModel>>(200, null, componentCollection);
            _apiClient.GetAllComponentsAsync().Returns(response);
            Services.AddSingleton(_apiClient);
        }
        
        [Fact]
        public async Task Binding_should_work()
        {
            await Dispatch(Component.Instance.AddressToVerifyTextBox.ValueChanged, "address");
            _componentInstance.AddressToVerify.Should().Be("address");

            await Dispatch(Component.Instance.ChosenCurrencySymbolTextBox.ValueChanged, "bat");
            _componentInstance.ChosenCurrencySymbol.Should().Be("bat");
        }

        [Fact]
        public async Task EditForm_should_be_valid_only_if_it_is_correctly_filled()
        {
            _componentInstance.EditContext.Validate().Should().BeFalse();

            _componentInstance.AddressToVerify = _mockCreator.GetRandomAddressEthereum();
            _componentInstance.ChosenCurrencySymbol = "bat";
            
            _componentInstance.EditContext.Validate().Should().BeTrue();
        }

        [Fact]
        public async Task EditForm_should_not_be_valid_the_address_is_not_an_ethereum_one()
        {
            _componentInstance.EditContext.Validate().Should().BeFalse();
            
            _componentInstance.AddressToVerify = _mockCreator.GetRandomAddressEthereum() + ".";
            _componentInstance.ChosenCurrencySymbol = "bat";
            
            _componentInstance.EditContext.Validate().Should().BeFalse();
        }
        
        [Fact]
        public async Task GetTrakxAddressAndVerificationAmount_should_save_correctly_trakx_address_and_verification_amount_on_view_model()
        {
            var symbol = _mockCreator.GetRandomString(3);
            var address = _mockCreator.GetRandomAddressEthereum();
            await Dispatch(_componentInstance.AddressToVerifyTextBox.ValueChanged, address);
            await Dispatch(_componentInstance.ChosenCurrencySymbolTextBox.ValueChanged, symbol);

            var trakxAddress = _mockCreator.GetRandomAddressEthereum();
            _addressMapping.GetTrakxAddress(symbol).ReturnsForAnyArgs(trakxAddress);
            var verificationAmount = _mockCreator.GetRandomPrice();
            _addressMapping.GetVerificationAmount(symbol).ReturnsForAnyArgs(verificationAmount);

            _componentInstance.EditContext.Validate().Should().BeTrue();
            await Component.TestContext.Renderer.Dispatcher.InvokeAsync(() =>
                Component.Instance.GetTrakxAddressAndVerificationAmount());

            _componentInstance.TrakxAddress.Should().Be(trakxAddress);
            _componentInstance.VerificationAmount.Should().Be(verificationAmount);
        }

        [Fact]
        public async Task Trakx_address_and_verification_amount_should_not_change_of_api_client_call_failed()
        {
            var symbol = _mockCreator.GetRandomString(3);
            var address = _mockCreator.GetRandomAddressEthereum();
            await Dispatch(_componentInstance.AddressToVerifyTextBox.ValueChanged, address);
            await Dispatch(_componentInstance.ChosenCurrencySymbolTextBox.ValueChanged, symbol);

            var exception = new ApiException("wrong way", 404, "You should not be here", null, null);
            _addressMapping.GetTrakxAddress(symbol).ThrowsForAnyArgs(exception);
            _addressMapping.GetVerificationAmount(symbol).ThrowsForAnyArgs(exception);

            _componentInstance.EditContext.Validate().Should().BeTrue();
            await Component.TestContext.Renderer.Dispatcher.InvokeAsync(() =>
                Component.Instance.GetTrakxAddressAndVerificationAmount());

            _componentInstance.TrakxAddress.Should().BeNullOrEmpty();
            _componentInstance.VerificationAmount.Should().Be(0m);
        }
    }
}