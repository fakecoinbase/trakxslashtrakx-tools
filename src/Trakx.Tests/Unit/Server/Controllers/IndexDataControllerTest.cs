using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Pricing;
using Trakx.Common.Interfaces.Index;
using Trakx.MarketData.Server.Models;
using NSubstitute;
using Trakx.Common.Core;
using Trakx.Common.Pricing;
using Trakx.MarketData.Server.Controllers;
using Trakx.Tests.Data;
using Xunit.Sdk;


namespace Trakx.Tests.Unit.Server.Controllers
{
    
    public class IndexDataControllerTest
    {
       private readonly IIndexDataProvider _indexProvider;
        private readonly INavCalculator _navCalculator;
        private readonly MockCreator _mockCreator;
        private readonly IndexDataController _indexDataController;


        public IndexDataControllerTest()
        {
            _indexProvider = Substitute.For<IIndexDataProvider>();
            _navCalculator = Substitute.For<INavCalculator>(); 
            _mockCreator = new MockCreator();
            _indexDataController= new IndexDataController(_indexProvider,_navCalculator, Substitute.For<IHostEnvironment>(), Substitute.For<ILogger<IndexDataController>>());
        }

        [Fact]
        public async Task IndexDetailsPrices()
        {
            var symbol = _mockCreator.GetRandomIndexSymbol();
            var composition = _mockCreator.GetIndexComposition(3);
            _indexProvider.GetCurrentComposition(symbol, default).ReturnsForAnyArgs(composition);

            var valuations = MockCreator.GetIndexValuation(composition);
            _indexProvider.GetInitialValuation(composition).Returns(valuations);
            _navCalculator.GetIndexValuation(composition).Returns(valuations);

            var result = await _indexDataController.IndexDetailsPriced(symbol);
            
            await _indexProvider.Received(1).GetInitialValuation(composition);
            IndexPricedModel finalResults = (IndexPricedModel)((JsonResult) result.Result).Value;
            finalResults.Address.Should().Be(composition.IndexDefinition.Address);
            finalResults.CreationDate.Should().Be(composition.IndexDefinition.CreationDate);
            finalResults.Symbol.ToLower().Should().Be(composition.IndexDefinition.Symbol);
            finalResults.Description.Should().Be(composition.IndexDefinition.Description);
            finalResults.Name.Should().Be(composition.IndexDefinition.Name);
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
        public async Task IndexDetailsPrices_if_composition_null()
        {
            var symbol = _mockCreator.GetRandomIndexSymbol();

            _indexProvider.GetCurrentComposition(symbol).ReturnsForAnyArgs((IIndexComposition)default);
            var result = await _indexDataController.IndexDetailsPriced(symbol);
            ((JsonResult)result.Result).Value.Should().Be($"failed to retrieve details for index {symbol}");
        }
    }
}
