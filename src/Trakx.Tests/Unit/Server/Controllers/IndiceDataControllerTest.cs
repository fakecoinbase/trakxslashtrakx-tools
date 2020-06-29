using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Pricing;
using Trakx.Common.Interfaces.Indice;
using Trakx.MarketData.Server.Models;
using NSubstitute;
using Trakx.MarketData.Server.Controllers;
using Trakx.Tests.Data;
using Xunit.Abstractions;


namespace Trakx.Tests.Unit.Server.Controllers
{
    
    public class IndiceDataControllerTest
    {
       private readonly IIndiceDataProvider _indiceProvider;
        private readonly INavCalculator _navCalculator;
        private readonly MockCreator _mockCreator;
        private readonly IndexDataController _indiceDataController;


        public IndiceDataControllerTest(ITestOutputHelper output)
        {
            _indiceProvider = Substitute.For<IIndiceDataProvider>();
            _navCalculator = Substitute.For<INavCalculator>(); 
            _mockCreator = new MockCreator(output);
            _indiceDataController= new IndexDataController(_indiceProvider,_navCalculator, Substitute.For<IHostEnvironment>(), 
                Substitute.For<ILogger<IndexDataController>>());
        }

        [Fact]
        public async Task IndiceDetailsPrices()
        {
            var symbol = _mockCreator.GetRandomIndiceSymbol();
            var composition = _mockCreator.GetIndiceComposition(3);
            _indiceProvider.GetCurrentComposition(symbol, default).ReturnsForAnyArgs(composition);

            var valuations = MockCreator.GetIndiceValuation(composition);
            _indiceProvider.GetInitialValuation(composition).Returns(valuations);
            _navCalculator.GetIndiceValuation(composition).Returns(valuations);

            var result = await _indiceDataController.IndexDetailsPriced(symbol);
            
            await _indiceProvider.Received(1).GetInitialValuation(composition);
            IndicePricedModel finalResults = (IndicePricedModel)((JsonResult) result.Result).Value;
            finalResults.Address.Should().Be(composition.IndiceDefinition.Address);
            finalResults.CreationDate.Should().Be(composition.CreationDate);
            finalResults.Symbol.ToLower().Should().Be(composition.IndiceDefinition.Symbol);
            finalResults.Description.Should().Be(composition.IndiceDefinition.Description);
            finalResults.Name.Should().Be(composition.IndiceDefinition.Name);
            finalResults.ComponentDefinitions.Count.Should().Be(3);
            finalResults.ComponentDefinitions[0].Address.Should()
                .Be(composition.ComponentQuantities[0].ComponentDefinition.Address);
            finalResults.ComponentDefinitions[0].Symbol.ToLower().Should()
                .Be(composition.ComponentQuantities[0].ComponentDefinition.Symbol);
            finalResults.CurrentValuation.ValuationsBySymbol[composition.ComponentQuantities[0].ComponentDefinition.Symbol.ToUpper()].Price.Should()
                .Be(valuations.ComponentValuations[0].Price);
            finalResults.InitialValuation.ValuationsBySymbol[composition.ComponentQuantities[0].ComponentDefinition.Symbol.ToUpper()].Price.Should()
                .Be(valuations.ComponentValuations[0].Price);
        }

        [Fact]
        public async Task IndiceDetailsPrices_if_composition_null()
        {
            var symbol = _mockCreator.GetRandomIndiceSymbol();

            _indiceProvider.GetCurrentComposition(symbol).ReturnsForAnyArgs((IIndiceComposition)default);
            var result = await _indiceDataController.IndexDetailsPriced(symbol);
            ((JsonResult)result.Result).Value.Should().Be($"failed to retrieve details for indice {symbol}");
        }
    }
}
