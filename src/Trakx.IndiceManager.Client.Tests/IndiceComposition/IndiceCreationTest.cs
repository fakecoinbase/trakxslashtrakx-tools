using System.Linq;
using FluentAssertions;
using Trakx.IndiceManager.Client.Pages.IndiceComposition;
using Xunit;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using MatBlazor;
using Trakx.IndiceManager.Client.Tests.Wrapping;

namespace Trakx.IndiceManager.Client.Tests.IndiceComposition
{
    public sealed class IndiceCreationTest: ComponentTestFixture
    {
        private readonly IRenderedComponent<IndiceCreation> _component;

        private IHtmlInputElement IndiceNameInput => (IHtmlInputElement)_component.FindElementWithId("input", "input-indice-name");
        private IHtmlInputElement IndiceSymbolInput => (IHtmlInputElement)_component.FindElementWithId("input", "input-indice-symbol");
        private IHtmlInputElement IndiceTargetedNavInput => (IHtmlInputElement)_component.FindElementWithId("input", "input-indice-targeted-nav");
        private IElement EqualWeightCheckBox => _component.FindElementWithId("input", "input-equal-weight");
        private IElement WeightInPercentageRadioButton => _component.FindElementWithId("input", "radio-weight-in-percentage");
        private IElement WeightInUsdcRadioButton => _component.FindElementWithId("input", "radio-weight-in-usdc");
        private IHtmlInputElement NewErc20SymbolInput => (IHtmlInputElement)_component.FindElementWithId("input", "input-add-erc20-symbol");
        private IHtmlInputElement NewErc20WeightInput => (IHtmlInputElement)_component.FindElementWithId("input", "input-add-erc20-weight");
        private IElement AddErc20Button => _component.FindElementWithId("input", "button-add-new-erc20");
        private IElement CalculateNavSubmitButton => _component.FindElementWithId("button", "button-submit-calculate-nav");
        private IMatToaster ToasterInstance => _component.Instance.Toaster;
        private IndiceCreation.IndiceCreationModel MyModelInstance =>
            (IndiceCreation.IndiceCreationModel) _component.Instance.myModel;
        
        
        public IndiceCreationTest()
        {
            Services.AddMatToaster();
            _component = RenderComponent<IndiceCreation>();
        }


        [Fact]
        public void add_a_new_erc20_to_component_list_should_failed_if_boxes_are_not_correctly_filled_and_if_the_component_is_already_in_the_list()
        {
            ToasterInstance.Toasts.Count.Should().Be(0);

            AddErc20Button.Click();
            ToasterInstance.Toasts.Single().Title.Should().Be("Error");
            ToasterInstance.Clear();

            NewErc20SymbolInput.Change("BAT");
            NewErc20WeightInput.Change("0.4");
            AddErc20Button.Click();
            ToasterInstance.Toasts.Single().Title.Should().Be("Success");
            ToasterInstance.Clear();

            NewErc20SymbolInput.Change("BAT");
            NewErc20WeightInput.Change("0.2");
            AddErc20Button.Click();
            ToasterInstance.Toasts.Single().Title.Should().Be("Error");
            ToasterInstance.Clear();

        }

        [Fact]
        public void successfull_submit_of_calculate_nav_form_should_register_correctly_the_inputs_in_the_model()
        {
            string expectedName = "test token";
            string expectedSymbol = "TT";
            double expectedTargetedNav = 123;
            string expectedErc20Symbol = "BAT";
            double expectedErc20Weight = 0.3;

            IndiceNameInput.Change(expectedName);
            IndiceSymbolInput.Change(expectedSymbol);
            IndiceTargetedNavInput.Change(expectedTargetedNav.ToString());
            NewErc20SymbolInput.Change(expectedErc20Symbol);
            NewErc20WeightInput.Change(expectedErc20Weight.ToString());
            AddErc20Button.Click();
            CalculateNavSubmitButton.Click();

            ToasterInstance.Toasts.Single().Title.Should().Be("Success");

            MyModelInstance.IndiceName.Should().Be(expectedName);
            MyModelInstance.IndiceSymbol.Should().Be(expectedSymbol);
            MyModelInstance.TargetedNav.Should().Be((decimal)expectedTargetedNav);
            MyModelInstance.ComponentList.Single().Symbol.Should().Be(expectedErc20Symbol);
            MyModelInstance.ComponentList.Single().Weight.Should().Be((decimal)expectedErc20Weight);

        }

        [Fact]
        public void click_on_weight_unit_in_percentage_should_unselect_usdc_option_and_vice_versa()
        {
            WeightInPercentageRadioButton.Click();
            WeightInPercentageRadioButton.IsChecked().Should().BeTrue();
            MyModelInstance.WeightUnit.Should().Be(IndiceCreation.IndiceCreationModel.WeightUnitType.Percentage);
            
            WeightInUsdcRadioButton.Click();
            WeightInPercentageRadioButton.IsChecked().Should().BeFalse();
            MyModelInstance.WeightUnit.Should().Be(IndiceCreation.IndiceCreationModel.WeightUnitType.USDc);

        }

        [Fact]
        public void check_box_equal_weight()
        {
            MyModelInstance.EqualWeights.Should().BeFalse();
            EqualWeightCheckBox.IsChecked().Should().BeFalse();

            EqualWeightCheckBox.Change(true);
            EqualWeightCheckBox.IsChecked().Should().BeTrue();
            MyModelInstance.EqualWeights.Should().BeTrue();

            EqualWeightCheckBox.Change(false);
            EqualWeightCheckBox.IsChecked().Should().BeFalse();
            MyModelInstance.EqualWeights.Should().BeFalse();
        }

    }

}
