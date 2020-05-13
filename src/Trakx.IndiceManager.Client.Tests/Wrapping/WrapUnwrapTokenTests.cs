using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Trakx.IndiceManager.Client.Pages.Wrapping;
using Xunit;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Components;
using Bunit;
using MatBlazor;
using Microsoft.AspNetCore.Components.Web;
using Trakx.Tests.Data;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Client.Tests.Wrapping
{
    public sealed class WrapUnwrapTokenTests : ComponentTestFixture
    {
        private readonly MockCreator _mockCreator;
        private readonly IRenderedComponent<WrapUnwrapToken> _component;
        private IHtmlInputElement AmountInput => (IHtmlInputElement) _component.FindElementWithId("input", "Amount");
        private IHtmlInputElement SendingAddressInput => (IHtmlInputElement)_component.FindElementWithId("input", "user-sending-address");
        private IHtmlInputElement ReceivingAddressInput => (IHtmlInputElement)_component.FindElementWithId("input", "user-receiving-address");
        private IElement WrapRadioButton => _component.FindElementWithId("input", "wrap-tokens");
        private IElement ResetButton => _component.FindElementWithId("input", "button-reset");
        private IElement UnwrapRadioButton => _component.FindElementWithId("input", "unwrap-tokens");
        private IHtmlSelectElement SelectMenu => (IHtmlSelectElement)_component.FindElementWithId("select", "select-menu");
        private IElement SubmitButton => _component.FindElementWithId("button", "button-submit");

        private List<IHtmlOptionElement> SourceCryptocurrencyOptions => _component.FindAll("option")
            .Where(o => o.Id?.StartsWith("source-") ?? false)
            .Cast<IHtmlOptionElement>()
            .ToList();


        private bool _submitted;


        public WrapUnwrapTokenTests(ITestOutputHelper output)
        {
            _mockCreator = new MockCreator(output);
            Services.AddMatToaster();
            _submitted = false;
            var submitEventCallback = EventCallback(nameof(WrapUnwrapToken.OnSubmitClick), (MouseEventArgs _) => _submitted = true);
            _component = RenderComponent<WrapUnwrapToken>(submitEventCallback);
        }

        [Fact]
        public void submit_click_is_available_only_if_one_of_wrap_or_unwrap_option_was_choose()
        {
            SubmitButton.Click();
            _submitted.Should().BeFalse();

            WrapRadioButton.Click();
            SubmitButton.Click();
            _submitted.Should().BeTrue();
        }

        [Fact]
        public void click_on_wrap_or_unwrap_button_should_display_the_corresponding_choice_in_the_submit_button()
        {
             WrapRadioButton.Click();
            SubmitButton.TextContent.Should().Be("Wrap");

            UnwrapRadioButton.Click();
            SubmitButton.TextContent.Should().Be("Unwrap");
        }

        [Fact]
        public void verify_that_form_boxes_are_disabled_when_no_wrapping_option_is_chosen_and_after_clicking_on_reset_button()
        {
            //initially, forms boxes should be disabled
            VerifyFormBoxesStates(true);

            //clicking on "wrap" button makes them all available
            WrapRadioButton.Click();
            VerifyFormBoxesStates(false);

            //clicking on "reset" button makes them all disabled again
            ResetButton.Click();
            VerifyFormBoxesStates(true);

            //clicking on "unwrap" button makes them all available
            WrapRadioButton.Click();
            VerifyFormBoxesStates(false);
        }

        private void VerifyFormBoxesStates(bool expectedDisabledState)
        {
            AmountInput.IsDisabled.Should().Be(expectedDisabledState);
            SendingAddressInput.IsDisabled.Should().Be(expectedDisabledState);
            ReceivingAddressInput.IsDisabled.Should().Be(expectedDisabledState);
            SelectMenu.IsDisabled.Should().Be(expectedDisabledState);
            SubmitButton.IsDisabled().Should().Be(expectedDisabledState);
        }

        [Fact]
        public void select_options_should_display_the_correct_list_of_cryptocurrencies_or_none_if_no_option_is_selected()
        {
            //initially it must not have any crypto currencies proposed
            SourceCryptocurrencyOptions[0].Text.Should().Be("Error: select an option above");
            SourceCryptocurrencyOptions.Count.Should().Be(1, "we only have one option when Database not connected.");

            //getting an instance of SubmitModel class of WrapUnwrapToken.razor
            var submitModelInstance = _component.Instance.myModel;

            //by clicking on the "wrap" button, only tokens from other blockchains than ethereum should be proposed (BTC, LTC etc...)
            WrapRadioButton.Click();
            SourceCryptocurrencyOptions.Select(o => o.Text).Should().BeEquivalentTo(submitModelInstance.wrappingOptions);

            //by clicking on the "unwrap" button, only tokens based on ethereum should be proposed (wrapped tokens)
            UnwrapRadioButton.Click();
            SourceCryptocurrencyOptions.Select(o => o.Text).Should().BeEquivalentTo(submitModelInstance.unwrappingOptions);

            //by clicking on the "reset" button, any crypto currencies should be proposed again
            ResetButton.Click();
            SourceCryptocurrencyOptions[0].Text.Should().Be("Error: select an option above");
            SourceCryptocurrencyOptions.Count.Should().Be(1, "we only have one option when Database not connected.");
        }

        [Fact]
        public void modification_on_forms_should_be_saved_on_model_attributes_and_reset_button_should_reset_every_inputs_and_model_variables()
        {
            var myModel = _component.Instance.myModel;

            //1) We test for "wrapping" option
            WrapRadioButton.Click();

            //todo: find a way to select an option 
            var selectMenu = SelectMenu;
            var btcOption = (IHtmlOptionElement)_component.FindElementWithId("option", "source-BTC");
            selectMenu.Options.SelectedIndex = selectMenu.Options.IndexOf(btcOption);

            //expected values
            var expectedAmount = 10;
            var expectedBtcAddress = "bc1qf749kttvwp8mjra55and424tf62hlngzu469t0"; //here we take "BTC" for example
            var expectedEthAddress = _mockCreator.GetRandomAddressEthereum();

            //changes on razor page...
            //todo: add test for <select> component
            AmountInput.Change(expectedAmount.ToString());
            SendingAddressInput.Change(expectedBtcAddress);
            ReceivingAddressInput.Change(expectedEthAddress);

            //...should be correctly saved on model
            //todo: add test for myModel.cryptocurrency
            myModel.Amount.Should().Be(expectedAmount);
            myModel.SendingAddress.Should().Be(expectedBtcAddress);
            myModel.ReceivingAddress.Should().Be(expectedEthAddress);


            //2) We reset the form and we verify that every model's variables are reset
            ResetButton.Click();

            //todo: add test for <select> component
            AmountInput.Value.Should().BeNullOrEmpty();
            SendingAddressInput.Value.Should().BeNullOrEmpty();
            ReceivingAddressInput.Value.Should().BeNullOrEmpty();

            //todo: add test for myModel.cryptocurrency
            myModel.Amount.Should().BeNull();
            myModel.SendingAddress.Should().BeNullOrEmpty();
            myModel.ReceivingAddress.Should().BeNullOrEmpty();


            //3) We test for "unwrapping" option
            UnwrapRadioButton.Click();

            //todo: add test for <select> component
            AmountInput.Change(expectedAmount.ToString());
            SendingAddressInput.Change(expectedEthAddress);
            ReceivingAddressInput.Change(expectedBtcAddress);

            //...should be correctly saved on model
            //todo: add test for myModel.cryptocurrency
            myModel.Amount.Should().Be(expectedAmount);
            myModel.SendingAddress.Should().Be(expectedEthAddress);
            myModel.ReceivingAddress.Should().Be(expectedBtcAddress);
        }

        public void click_on_submit_form_should_failed_if_not_all_boxes_are_not_correctly_filled_and_succeed_if_everything_is_ok()
        {
            //todo: This test will check that submission is succeed only if everything is well filled.
        }

        public void click_on_reset_button_should_display_a_reset_event()
        {
            //todo: This test will check that reset button display a corresponding event.
        }
    }

    public static class RenderedComponentExtension
    {
        public static IElement FindElementWithId<T>(this IRenderedComponent<T> component, string type, string id)
            where T : class, IComponent
        {
            return component.FindAll(type).Single(c => c.Id == id);
        }
    }
}