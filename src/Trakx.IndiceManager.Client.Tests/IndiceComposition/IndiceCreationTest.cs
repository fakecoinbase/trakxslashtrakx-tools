using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.IndiceManager.Client.Pages.IndiceComposition;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Grids;
using Trakx.Common.Models;

namespace Trakx.IndiceManager.Client.Tests.IndiceComposition
{
    public sealed class IndiceCreationTest : ComponentTest<IndiceCreation>
    {
        private readonly IndiceCreation.IndiceCreationViewModel _model;
        private readonly IndiceCreation.ConstituentModel _constituent;
        private readonly MockCreator _mockCreator;
        private readonly IIndiceCreationClient _apiClient;

        public IndiceCreationTest(ITestOutputHelper output)
        {
            Services.AddSyncfusionBlazor();
            Services.AddSingleton(Substitute.For<ILogger<IndiceCreation>>());
            _apiClient = Substitute.For<IIndiceCreationClient>();
            Services.AddSingleton(_apiClient);
            Component = RenderComponent<IndiceCreation>();
            _mockCreator = new MockCreator(output);
            
            _model = Component.Instance.Model;
            _constituent = new IndiceCreation.ConstituentModel { Symbol = "ZZZ", Weight = 0.15m };
        }

        [Fact]
        public async Task Changing_symbol_in_textBox_should_change_Model()
        {
            await Dispatch(Component.Instance.SymbolTextBox.ValueChanged, "symbol");
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
            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged, IndiceCreation.IndiceCreationViewModel.WeightUnitType.USDc);
            _model.WeightUnit.Should().Be(IndiceCreation.IndiceCreationViewModel.WeightUnitType.USDc);

            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged, IndiceCreation.IndiceCreationViewModel.WeightUnitType.Percentage);
            _model.WeightUnit.Should().Be(IndiceCreation.IndiceCreationViewModel.WeightUnitType.Percentage);

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
                Data = new IndiceCreation.ConstituentModel { Symbol = _constituent.Symbol, Weight = _constituent.Weight + 0.2m },
                PreviousData = _constituent,
                RequestType = Action.Save
            };

            await Component.Instance.CheckRowChange(newChange);
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

        [Fact]
        public async Task ToIndiceCompositionModel_should_work()
        {
            var indiceDefinition = _mockCreator.GetRandomIndiceDefinition();

            _model.IndiceName = indiceDefinition.Name;
            _model.IndiceSymbol = indiceDefinition.Symbol;
            _model.IndiceDescription = indiceDefinition.Description;
            _model.Components.Add(_constituent);

            await Dispatch(Component.Instance.SaveIndiceButton.OnClick, new MouseEventArgs()).ConfigureAwait(false);

            await _apiClient.Received(1).SaveIndiceCompositionAsync(
                Arg.Is<IndiceCompositionModel>(m =>
                    m.IndiceDetail.Symbol == _model.IndiceSymbol
                    && m.IndiceDetail.Name == _model.IndiceName
                    && m.IndiceDetail.Description == _model.IndiceDescription
                    && m.Components[0].Symbol == _constituent.Symbol),
                Arg.Any<CancellationToken>()).ConfigureAwait(false);


        }
    }
}
