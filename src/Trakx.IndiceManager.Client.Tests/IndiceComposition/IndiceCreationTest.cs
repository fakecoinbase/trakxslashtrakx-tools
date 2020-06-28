using System.Collections.Generic;
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
using System.Linq;
using Trakx.Common.Composition;
using Trakx.IndiceManager.ApiClient;

namespace Trakx.IndiceManager.Client.Tests.IndiceComposition
{
    public sealed class IndiceCreationTest : ComponentTest<IndiceCreation>
    {
        private readonly IndiceCreation.ConstituentRow _constituent;
        private readonly MockCreator _mockCreator;
        private readonly IIndiceCreationClient _apiClient;
        private readonly IWeightCalculator _weightCalculator;
        private readonly IndiceCreation _componentInstance;

        public IndiceCreationTest(ITestOutputHelper output)
        {
            Services.AddSyncfusionBlazor();
            Services.AddSingleton(Substitute.For<ILogger<IndiceCreation>>());

            _apiClient = Substitute.For<IIndiceCreationClient>();
            _weightCalculator = Substitute.For<IWeightCalculator>();
            _mockCreator = new MockCreator(output);
            var componentDetail = new ComponentDetailModel(_mockCreator.GetComponentQuantity());
            var componentCollection = new List<ComponentDetailModel>();
            componentCollection.Add(componentDetail);
            var response = new Response<List<ComponentDetailModel>>(200, null, componentCollection);
            _apiClient.GetAllComponentsAsync().Returns(response);

            Services.AddSingleton(_apiClient);
            Services.AddSingleton(_weightCalculator);
            Component = RenderComponent<IndiceCreation>();
            _componentInstance = Component.Instance;

            _constituent = new IndiceCreation.ConstituentRow { Symbol = componentDetail.Symbol, CustomWeight = 0.15m };
        }

        [Fact]
        public async Task Changing_symbol_in_textBox_should_change_Model()
        {
            await Dispatch(Component.Instance.SymbolTextBox.ValueChanged, "symbol");
            _componentInstance.IndexSymbol.Should().Be("symbol");
        }

        [Fact]
        public async Task Changing_name_in_textBox_should_change_Model()
        {
            await Dispatch(Component.Instance.NameTextBox.ValueChanged, "name");
            _componentInstance.IndexName.Should().Be("name");
        }

        [Fact]
        public async Task Changing_targetNav_in_textBox_should_change_Model()
        {
            await Dispatch(Component.Instance.TargetNavTextBox.ValueChanged, 123.45m);
            _componentInstance.TargetedNav.Should().Be(123.45m);
        }

        [Fact]
        public async Task Clicking_equal_weight_should_change_Model()
        {
            await Dispatch(Component.Instance.EqualWeightCheckBox.CheckedChanged, true);
            _componentInstance.EqualWeightCheckBox.Checked.Should().Be(true);

            await Dispatch(Component.Instance.EqualWeightCheckBox.CheckedChanged, false);
            _componentInstance.EqualWeightCheckBox.Checked.Should().Be(false);
        }

        [Fact]
        public async Task Changing_weight_units_should_change_Model()
        {
            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged, IndiceCreation.WeightUnitType.USDc);
            _componentInstance.WeightUnit.Should().Be(IndiceCreation.WeightUnitType.USDc);

            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged, IndiceCreation.WeightUnitType.Percentage);
            _componentInstance.WeightUnit.Should().Be(IndiceCreation.WeightUnitType.Percentage);

        }

        [Fact]
        public async Task DataGrid_should_be_bound_to_Model_component_list()
        {
            _componentInstance.ConstituentsGrid.DataSource.Should().BeEquivalentTo(Component.Instance.ConstituentRows);
        }

        [Fact]
        public async Task Adding_new_component_should_fail_if_component_already_added()
        {
            Component.Instance.ConstituentRows.Add(_constituent);

            var newChange = new ActionEventArgs<IndiceCreation.ConstituentRow>
            {
                Data = _constituent,
                PreviousData = new IndiceCreation.ConstituentRow(),
                RequestType = Syncfusion.Blazor.Grids.Action.Save
            };
            
            await Component.TestContext.Renderer.Dispatcher.InvokeAsync(() =>
                Component.Instance.CheckRowChange(newChange));
            newChange.Cancel.Should().BeTrue();
        }

        [Fact]
        public async Task Changing_existing_component_should_be_allowed()
        {
            var newChange = new ActionEventArgs<IndiceCreation.ConstituentRow>
            {
                Data = new IndiceCreation.ConstituentRow { Symbol = _constituent.Symbol, CustomWeight = _constituent.CustomWeight + 0.2m },
                PreviousData = _constituent,
                RequestType = Syncfusion.Blazor.Grids.Action.Save
            };

            await Component.Instance.CheckRowChange(newChange);
            newChange.Cancel.Should().BeFalse();
        }

        [Fact]
        public void CalculateNav_should_require_one_constituent_in_model()
        {
            _componentInstance.ConstituentRows.Should().BeEmpty();
            _componentInstance.CalculateNavButton.Disabled.Should().BeTrue();

            _componentInstance.ConstituentRows.Add(_constituent);
            Component.Render();
            _componentInstance.CalculateNavButton.Disabled.Should().BeFalse();

        }

        [Fact]
        public void SymbolGridColumn_should_be_primary_key()
        {
            _componentInstance.SymbolGridColumn.IsPrimaryKey.Should()
                .BeTrue("otherwise you can't really delete records once inserted");
        }

        [Fact]
        public async Task Conversion_to_IndiceCompositionModel_should_work()
        {
            var indiceDefinition = _mockCreator.GetRandomIndiceDefinition();
            
            _componentInstance.IndexName = indiceDefinition.Name;
            _componentInstance.IndexSymbol = indiceDefinition.Symbol;
            _componentInstance.IndexDescription = indiceDefinition.Description;
            _componentInstance.ConstituentRows.Add(_constituent);

            await Dispatch(_componentInstance.SaveIndiceButton.OnClick, new MouseEventArgs());

            await _apiClient.Received(1).SaveIndiceCompositionAsync(
                Arg.Is<IndiceCompositionModel>(m =>
                    m.IndiceDetail.Symbol == indiceDefinition.Symbol
                    && m.IndiceDetail.Name == indiceDefinition.Name
                    && m.IndiceDetail.Description == indiceDefinition.Description
                    && m.Components[0].Symbol == _constituent.Symbol));
        }

        [Fact]
        public async Task ViewModel_should_be_empty_after_saved()
        {
            var indiceDefinition = _mockCreator.GetRandomIndiceDefinition();

            _componentInstance.IndexName = indiceDefinition.Name;
            _componentInstance.IndexSymbol = indiceDefinition.Symbol;
            _componentInstance.IndexDescription = indiceDefinition.Description;
            Component.Instance.ConstituentRows.Add(_constituent);

            await Dispatch(Component.Instance.SaveIndiceButton.OnClick, new MouseEventArgs());

            _componentInstance.IndexSymbol.Should().BeNull();
            _componentInstance.IndexDescription.Should().BeNull();
            _componentInstance.IndexName.Should().BeNull();
            _componentInstance.TargetedNav.Should().BeNull();
            _componentInstance.ConstituentRows.Should().BeNullOrEmpty();

        }

        [Fact]
        public void Switching_WeightUnit_to_percentage_should_automatically_compute_weights()
        {
            var constituent1 = new IndiceCreation.ConstituentRow { Symbol = "ABC", CustomWeight = 80m };
            var constituent2 = new IndiceCreation.ConstituentRow { Symbol = "DEF", CustomWeight = 20m };
            _componentInstance.ConstituentRows.Add(constituent1);
            _componentInstance.ConstituentRows.Add(constituent2);

            _componentInstance.WeightUnit = IndiceCreation.WeightUnitType.Percentage;

            _componentInstance.ConstituentRows.Where(o => o.Symbol == constituent1.Symbol).Select(o => o.CustomWeight.Should().Be(0.8m));
            _componentInstance.ConstituentRows.Where(o => o.Symbol == constituent2.Symbol).Select(o => o.CustomWeight.Should().Be(0.2m));
        }

        [Fact]
        public void Switching_WeightUnit_to_usdc_should_automatically_compute_UsdcValues()
        {
            var constituent1 = new IndiceCreation.ConstituentRow { Symbol = "ABC", CustomWeight = 0.123m };
            var constituent2 = new IndiceCreation.ConstituentRow { Symbol = "DEF", CustomWeight = 0.877m };
            _componentInstance.ConstituentRows.Add(constituent1);
            _componentInstance.ConstituentRows.Add(constituent2);

            _componentInstance.TargetedNav = 100m;
            _componentInstance.WeightUnit = IndiceCreation.WeightUnitType.USDc;

            _componentInstance.ConstituentRows.Where(o => o.Symbol == constituent1.Symbol).Select(o => o.CustomWeight.Should().Be(12.3m));
            _componentInstance.ConstituentRows.Where(o => o.Symbol == constituent2.Symbol).Select(o => o.CustomWeight.Should().Be(87.7m));
        }

        [Fact]
        public async Task Equal_weights_should_refresh_weights()
        {
            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged,
                IndiceCreation.WeightUnitType.Percentage);
            _componentInstance.ConstituentRows.Add(_constituent);

            var returnedDictionnary = new Dictionary<string, decimal>

            {
                {_componentInstance.ConstituentRows.First().Symbol,1m}
            };

            _weightCalculator.DistributeWeights(_componentInstance.ConstituentRows
                    .Select(c => c.Symbol)
                    .ToList())
                .ReturnsForAnyArgs(returnedDictionnary);

            await Dispatch(Component.Instance.EqualWeightCheckBox.CheckedChanged, true);
            
            _componentInstance.ConstituentRows.First().EqualWeight.Should().Be(1m);
        }

        [Fact]
        public async Task WeightRefresh_for_unit_in_usdc_should_calculate_weights_and_nav_from_usdc_values()
        {
            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged, IndiceCreation.WeightUnitType.USDc);

            _componentInstance.ConstituentRows.Add(_constituent);

            var returnedDictionnary = new Dictionary<string, decimal>

            {
                {_componentInstance.ConstituentRows.First().Symbol,_componentInstance.ConstituentRows.First().Value ?? 0m}
            };
            var componentsDictionnary = _componentInstance.ConstituentRows.ToDictionary(x => x.Symbol, x => x.Value ?? 0);
            
            _weightCalculator.CalculateWeightsFromUsdcValues(componentsDictionnary)
                .ReturnsForAnyArgs(returnedDictionnary);

            Component.Instance.RefreshWeights();


            _weightCalculator.Received(1).CalculateWeightsFromUsdcValues(
                Arg.Is<Dictionary<string, decimal>>(m =>
                    m[_componentInstance.ConstituentRows[0].Symbol] == (_componentInstance.ConstituentRows[0].Value ?? 0m)));
        }

        [Fact]
        public async Task WeightRefresh_for_unit_in_percentage_should_calculate_usdc_values_from_nav_and_weights()
        {
            await Dispatch(Component.Instance.WeightUnitsDropdown.ValueChanged, IndiceCreation.WeightUnitType.Percentage);

            _componentInstance.ConstituentRows.Add(_constituent);
            _componentInstance.TargetedNav = _mockCreator.GetRandomPrice();

            var returnedDictionnary = new Dictionary<string, decimal>()
            {
                {_componentInstance.ConstituentRows.First().Symbol,_componentInstance.ConstituentRows.First().CustomWeight ?? 0m}
            };
            var componentsDictionnary = _componentInstance.ConstituentRows.ToDictionary(x => x.Symbol, x => x.Value ?? 0);

            _weightCalculator.CalculateUsdcValuesFromNavAndWeights(componentsDictionnary,_componentInstance.TargetedNav ?? 0m)
                .ReturnsForAnyArgs(returnedDictionnary);

            Component.Instance.RefreshWeights();

            _weightCalculator.Received(1).CalculateUsdcValuesFromNavAndWeights(
                Arg.Is<Dictionary<string,decimal>> (d =>
                    d[_componentInstance.ConstituentRows[0].Symbol] == (_componentInstance.ConstituentRows[0].CustomWeight ?? 0m))
                , Arg.Is<decimal>( n => n == (_componentInstance.TargetedNav ?? 0m)));
        }


    }

}
