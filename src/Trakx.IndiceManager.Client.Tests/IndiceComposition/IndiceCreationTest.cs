
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.IndiceManager.Client.Pages.IndiceComposition;
using Xunit;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;

namespace Trakx.IndiceManager.Client.Tests.IndiceComposition
{
    public sealed class IndiceCreationTest: ComponentTest<IndiceCreation>
    { 
        private readonly IndiceCreation.IndiceCreationModel _model;
        private readonly IndiceCreation.ConstituentModel _constituent;

        public IndiceCreationTest()
        {
            Services.AddSyncfusionBlazor();
            Services.AddSingleton(Substitute.For<ILogger<IndiceCreation>>());
            Component = RenderComponent<IndiceCreation>();

            _model = Component.Instance.Model;
            _constituent = new IndiceCreation.ConstituentModel {Symbol = "ZZZ", Weight = 0.15m};
        }

        [Fact]
        public async Task Changing_symbol_in_textBox_should_change_Model()
        {
            await Dispatch(Component.Instance.SymbolTextBox.ValueChanged,"symbol");
            _model.IndiceSymbol.Should().Be("symbol");
        }

        [Fact]
        public async Task Changing_name_in_textBox_should_change_Model()
        {
            await Dispatch(Component.Instance.NameTextBox.ValueChanged, "name");
            _model.IndiceName.Should().Be("name");
        }

        [Fact]
        public async Task Changing_targetNav_in_textBox_should_change_Model()
        {
            await Dispatch(Component.Instance.TargetNavTextBox.ValueChanged, 123.45m);
            _model.TargetedNav.Should().Be(123.45m);
        }

        [Fact]
        public async Task Clicking_equal_weight_should_change_Model()
        {
            await Dispatch(Component.Instance.EqualWeightSwitch.CheckedChanged, true);
            _model.EqualWeights.Should().BeTrue();

            await Dispatch(Component.Instance.EqualWeightSwitch.CheckedChanged, false);
            _model.EqualWeights.Should().BeFalse();
        }


        [Fact]
        public async Task Changing_weight_units_should_change_Model()
        {
            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged, IndiceCreation.IndiceCreationModel.WeightUnitType.USDc);
            _model.WeightUnit.Should().Be(IndiceCreation.IndiceCreationModel.WeightUnitType.USDc);

            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged, IndiceCreation.IndiceCreationModel.WeightUnitType.Percentage);
            _model.WeightUnit.Should().Be(IndiceCreation.IndiceCreationModel.WeightUnitType.Percentage);

        }

        [Fact]
        public async Task DataGrid_should_be_bound_to_Model_component_list()
        {
            Component.Instance.ConstituentsGrid.DataSource.Should().BeSameAs(Component.Instance.Model.Components);
        }

        [Fact]
        public async Task Adding_new_component_should_fail_if_component_already_added()
        {
            _model.Components.Add(_constituent);

            var newChange = new ActionEventArgs<IndiceCreation.ConstituentModel>
            {
                Data = _constituent,
                PreviousData = new IndiceCreation.ConstituentModel(),
                RequestType = Action.Save
            };

            await Component.TestContext.Renderer.Dispatcher.InvokeAsync(() =>
                Component.Instance.CheckRowChange(newChange));
            newChange.Cancel.Should().BeTrue();
        }

        [Fact]
        public async Task Changing_existing_component_should_be_allowed()
        {
            var newChange = new ActionEventArgs<IndiceCreation.ConstituentModel>
            {
                Data = new IndiceCreation.ConstituentModel { Symbol= _constituent.Symbol, Weight = _constituent.Weight + 0.2m },
                PreviousData = _constituent,
                RequestType = Action.Save
            };

            Component.Instance.CheckRowChange(newChange);
            newChange.Cancel.Should().BeFalse();
        }

        [Fact]
        public void CalculateNav_should_require_one_constituent_in_model()
        {
            _model.Components.Should().BeEmpty();
            Component.Instance.CalculateNavButton.Disabled.Should().BeTrue();

            _model.Components.Add(_constituent);
            Component.Render();
            Component.Instance.CalculateNavButton.Disabled.Should().BeFalse();

        }

        [Fact]
        public void SymbolGridColumn_should_be_primary_key()
        {
            Component.Instance.SymbolGridColumn.IsPrimaryKey.Should()
                .BeTrue("otherwise you can't really delete records once inserted");
        }
    }
}
