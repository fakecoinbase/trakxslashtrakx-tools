using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using FluentAssertions;
using Trakx.IndiceManager.Client.Pages.Wrapping;
using Trakx.Tests.Data;
using Trakx.Common.Models;
using Xunit;
using Xunit.Abstractions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Components;
using Bunit;
using Syncfusion.Blazor;
using Flurl.Http.Testing;
using Flurl.Http;
using Trakx.Persistence.Tests;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Trakx.IndiceManager.ApiClient;

namespace Trakx.IndiceManager.Client.Tests.Wrapping
{
    public sealed class WrapUnwrapTokenTests : ComponentTest<WrapUnwrapToken>
    {
        private readonly MockCreator _mockCreator;
        private readonly WrapUnwrapToken.WrappingTransactionViewModel _model;
        private readonly HttpTest _httpTest;

        public WrapUnwrapTokenTests(ITestOutputHelper output)
        {
            _mockCreator = new MockCreator(output);
            Services.AddSyncfusionBlazor();
            Component = RenderComponent<WrapUnwrapToken>();
            _mockCreator = new MockCreator(output);
            _httpTest = new HttpTest();

            _model = Component.Instance.Model;
        }

        [Fact]
        public void Wrapping_should_be_the_default_option()
        {
            _model.Wrapping.Should().BeTrue();
            Component.Instance.WrappingRadioButton.Checked.Should().BeTrue();
        }

        [Fact]
        public async Task Wrapping_should_determinate_list_of_from_currencies()
        {
            _model.Wrapping.Should().BeTrue();
            Component.Instance.FromCurrencyTextBox.DataSource.Any(c => c.StartsWith("w")).Should().BeFalse();

            await Dispatch(Component.Instance.WrappingRadioButton.CheckedChanged, false);
            Component.Instance.FromCurrencyTextBox.DataSource.All(c => c.StartsWith("w")).Should().BeTrue();
        }

        [Fact]
        public async Task FromCurrencyTextBox_should_change_model_from_Currency()
        {
            _model.FromCurrency.Should().BeNullOrEmpty();
            var selectedCurrency = Component.Instance.CurrencyOptions[1];
            await Dispatch(Component.Instance.FromCurrencyTextBox.ValueChanged, selectedCurrency);
            _model.FromCurrency.Should().Be(selectedCurrency);
        }

        [Fact]
        public async Task FromCurrencyTextBox_should_preserve_value_on_change_of_wrapping_operation()
        {
            var currencyToWrap = Component.Instance.CurrencyOptions[0];
            _model.FromCurrency = currencyToWrap;
            Component.Instance.FromCurrencyTextBox.Index = 0;

            await Dispatch(Component.Instance.WrappingRadioButton.CheckedChanged, false);
            Component.Render();
            var currencyToUnwrap = "w" + currencyToWrap;
            _model.FromCurrency.Should().Be(currencyToUnwrap);

            await Dispatch(Component.Instance.WrappingRadioButton.CheckedChanged, true);
            Component.Render();
            _model.FromCurrency.Should().Be(currencyToWrap);
        }

        [Fact]
        public async Task AmountTextBox_should_be_bound_to_model_Amount()
        {
            _model.Amount.Should().BeNull("we start with an empty model");
            await Dispatch(Component.Instance.AmountTextBox.ValueChanged, 123.45m);
            _model.Amount.Should().Be(123.45m);
        }

        [Fact]
        public async Task FromAddress_and_ToAddress_should_bind_to_correct_addresses_when_wrapping()
        {
            _model.Wrapping.Should().BeTrue("we start with wrapping mode");

            var wrappingFrom = "wrappingFrom";
            await Dispatch(Component.Instance.WrappingFromAddress.ValueChanged, wrappingFrom);
            OnlyNativeAddressShouldBe(wrappingFrom);

            var wrappingTo = "wrappingTo";
            await Dispatch(Component.Instance.WrappingToAddress.ValueChanged, wrappingTo);
            OnlyEthereumAddressShouldBe(wrappingTo);
        }


        [Fact]
        public async Task FromAddress_and_ToAddress_should_bind_to_correct_addresses_when_unwrapping()
        {
            _model.Wrapping = false;
            Component.Render();

            var unwrappingFrom = "unwrappingFrom";
            await Dispatch(Component.Instance.UnwrappingFromAddress.ValueChanged, unwrappingFrom);
            OnlyEthereumAddressShouldBe(unwrappingFrom);

            var unwrappingTo = "unwrappingTo";
            await Dispatch(Component.Instance.UnwrappingToAddress.ValueChanged, unwrappingTo);
            OnlyNativeAddressShouldBe(unwrappingTo);
        }

        private void OnlyEthereumAddressShouldBe(string expectedEthereumAddress)
        {
            _model.EthereumAddress.Should().Be(expectedEthereumAddress);
            _model.NativeAddress.Should().NotBe(expectedEthereumAddress);
        }

        private void OnlyNativeAddressShouldBe(string expectedNativeAddress)
        {
            _model.EthereumAddress.Should().NotBe(expectedNativeAddress);
            _model.NativeAddress.Should().Be(expectedNativeAddress);
        }

        [Fact]
        public async Task NativeAddress_and_EthereumAddress_should_not_change_on_wrapping_operation_change()
        {
            await FromAddress_and_ToAddress_should_bind_to_correct_addresses_when_wrapping().ConfigureAwait(false);
            var ethereumAddress = _model.EthereumAddress;
            var nativeAddress = _model.NativeAddress;

            await Dispatch(Component.Instance.WrappingRadioButton.CheckedChanged, false);
            _model.Wrapping.Should().BeFalse();

            _model.EthereumAddress.Should().Be(ethereumAddress);
            _model.NativeAddress.Should().Be(nativeAddress);
        }

        [Fact]
        public async Task IsSubmitDisabled_should_be_true_until_model_is_valid()
        {
            (await Dispatch(() => Component.Instance.EditContext.Validate()))
                .Should().BeFalse();

            var currencyToWrap = Component.Instance.CurrencyOptions[0];
            _model.FromCurrency = currencyToWrap;
            (await Dispatch(() => Component.Instance.EditContext.Validate()))
                .Should().BeFalse();

            _model.Amount = 123.45m;
            (await Dispatch(() => Component.Instance.EditContext.Validate()))
                .Should().BeFalse();

            _model.EthereumAddress = _mockCreator.GetRandomAddressEthereum() + "notvalid";
            (await Dispatch(() => Component.Instance.EditContext.Validate()))
                .Should().BeFalse();

            _model.NativeAddress = "valid";
            (await Dispatch(() => Component.Instance.EditContext.Validate()))
                .Should().BeFalse();

            _model.EthereumAddress = _mockCreator.GetRandomAddressEthereum();
            (await Dispatch(() => Component.Instance.EditContext.Validate()))
                .Should().BeFalse();

            _model.Amount = 3.45m;
            (await Dispatch(() => Component.Instance.EditContext.Validate()))
                .Should().BeTrue();
        }

        [Fact]
        public async Task Reset_should_create_new_Model_and_EditContext()
        {
            var editContext = Component.Instance.EditContext;
            editContext.Model.Should().BeSameAs(_model);

            await Dispatch(Component.Instance.ResetButton.OnClick,
                new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            var newModel = Component.Instance.Model;
            newModel.Should().NotBeSameAs(_model);

            var newEditContext = Component.Instance.EditContext;
            newEditContext.Should().NotBeSameAs(editContext);
            newEditContext.Model.Should().BeSameAs(newModel);
        }

        [Fact]
        public async Task ToWrappingTransactionModel_should_work()
        {
            string fakeHttp = "https://localhost:44373/";

            var currencyToWrap = Component.Instance.CurrencyOptions[0];
            _model.FromCurrency = currencyToWrap;
            _model.NativeAddress = "valid";
            _model.EthereumAddress = _mockCreator.GetRandomAddressEthereum();
            _model.Amount = 3.45m;
            (await Dispatch(() => Component.Instance.EditContext.Validate()))
                .Should().BeTrue();

            WrappingTransactionModel wrappingTransaction = _model.ToWrappingTransactionModel();

            string jsonModel = JsonSerializer.Serialize(wrappingTransaction);

            _httpTest.RespondWith(jsonModel);

            var apiResponse = await fakeHttp.GetJsonAsync();
            var jsonApiResponse = JsonSerializer.Serialize(apiResponse);

            WrappingTransactionModel objResult = JsonSerializer.Deserialize<WrappingTransactionModel>(jsonApiResponse);

            objResult.Should().BeEquivalentTo(wrappingTransaction);

        }
    }
}