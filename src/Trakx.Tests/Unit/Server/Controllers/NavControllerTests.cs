using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Pricing;
using Trakx.Common.Pricing;
using Trakx.MarketData.Server.Controllers;
using Trakx.MarketData.Server.Models;
using Trakx.Persistence.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Server.Controllers
{
    public class NavControllerTests
    {
        private const string InvalidIndiceOrCompositionSymbol = "abcdef";
        private readonly IIndiceDataProvider _indiceProvider;
        private readonly INavCalculator _navCalculator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly NavController _navController;
        private readonly MockCreator _mockCreator;
        private readonly DateTime _startTime;
        private readonly DateTime _endTime;

        public NavControllerTests(ITestOutputHelper output)
        {
            _indiceProvider = Substitute.For<IIndiceDataProvider>();
            _navCalculator = Substitute.For<INavCalculator>();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _navController = new NavController(_indiceProvider, _navCalculator, _dateTimeProvider,
                Substitute.For<ILogger<NavController>>());
            _mockCreator = new MockCreator(output);
            _startTime = _mockCreator.GetRandomDateTime();
            _endTime = _startTime.AddDays(50);
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_look_for_indice_composition_at_date_if_given_indice_symbol()
        {
            var symbol = _mockCreator.GetRandomIndiceSymbol();
            var cancellationToken = new CancellationToken();

            _indiceProvider.GetCompositionAtDate(default, default, default)
                .ReturnsForAnyArgs((IIndiceComposition)default);
            var result = await _navController.GetUsdNetAssetValue(symbol, cancellationToken: cancellationToken);

            await _indiceProvider.Received(1)
                .GetCompositionAtDate(Arg.Is(symbol), Arg.Any<DateTime>(), Arg.Is(cancellationToken));
            await _indiceProvider.DidNotReceiveWithAnyArgs().GetCompositionFromSymbol(default, default);

            ((JsonResult)result.Result).Value.Should().Be($"failed to retrieve composition for indice {symbol}.");
        }

        [Fact]
        public async Task
            GetUsdNetAssetValue_should_look_for_indice_composition_from_symbol_if_given_composition_symbol()
        {
            var symbol = _mockCreator.GetRandomCompositionSymbol();
            var cancellationToken = new CancellationToken();
            var result = await _navController.GetUsdNetAssetValue(symbol, cancellationToken: cancellationToken);

            await _indiceProvider.DidNotReceiveWithAnyArgs().GetCompositionAtDate(default, default, cancellationToken);
            await _indiceProvider.Received(1).GetCompositionFromSymbol(Arg.Is(symbol), Arg.Is(cancellationToken));
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_return_immediately_on_bad_symbols()
        {
            var cancellationToken = new CancellationToken();
            var result = await _navController.GetUsdNetAssetValue(InvalidIndiceOrCompositionSymbol,
                cancellationToken: cancellationToken);

            await _indiceProvider.DidNotReceiveWithAnyArgs().GetCompositionAtDate(default, default, cancellationToken);
            await _navCalculator.DidNotReceiveWithAnyArgs().GetIndiceValuation(default, default);

            ((JsonResult)result.Result).Value.Should()
                .Be($"{InvalidIndiceOrCompositionSymbol} is not a valid symbol.");
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_retrieve_composition_asOf_utcNow_if_compositionAsOf_unspecified()
        {
            var cancellationToken = new CancellationToken();
            var utcNow = new DateTime(2020, 02, 29);
            var priceAsOf = new DateTime(2020, 02, 10);
            _dateTimeProvider.UtcNow.Returns(utcNow);

            var composition = _mockCreator.GetIndiceComposition(2);
            _indiceProvider.GetCompositionAtDate(Arg.Any<string>(), utcNow, cancellationToken)
                .Returns(composition);

            var mockValuation = Substitute.For<IIndiceValuation>();
            var nav = 123.456m;
            mockValuation.NetAssetValue.Returns(nav);
            _navCalculator.GetIndiceValuation(composition, priceAsOf).Returns(mockValuation);

            var indiceSymbol = _mockCreator.GetRandomIndiceSymbol();

            var result = await _navController.GetUsdNetAssetValue(indiceSymbol,
                priceAsOf, default, 0, cancellationToken);

            await _indiceProvider.Received(1).GetCompositionAtDate(indiceSymbol, utcNow, cancellationToken);
            await _navCalculator.Received(1).GetIndiceValuation(composition, priceAsOf);

            ((JsonResult)result.Result).Value.Should().Be(nav);
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_retrieve_prices_asOf_utcNow_if_pricesAsOf_unspecified()
        {
            var cancellationToken = new CancellationToken();
            var valuationAsOf = default(DateTime?);
            var compositionAsOf = new DateTime(2020, 03, 01);

            var composition = _mockCreator.GetIndiceComposition(3);
            _indiceProvider.GetCompositionAtDate(Arg.Any<string>(), compositionAsOf, cancellationToken)
                .Returns(composition);

            var mockValuation = Substitute.For<IIndiceValuation>();
            var nav = 78.9m;
            mockValuation.NetAssetValue.Returns(nav);
            _navCalculator.GetIndiceValuation(composition, valuationAsOf).Returns(mockValuation);

            var indiceSymbol = _mockCreator.GetRandomIndiceSymbol();

            var result = await _navController.GetUsdNetAssetValue(indiceSymbol,
                default, compositionAsOf, 0, cancellationToken);

            await _indiceProvider.Received(1).GetCompositionAtDate(indiceSymbol, compositionAsOf, cancellationToken);
            await _navCalculator.Received(1).GetIndiceValuation(composition, default);

            ((JsonResult)result.Result).Value.Should().Be(nav);
        }

        [Fact]
        public async Task GetHistoricalUsdcNetAssetValues_should_fail_on_bad_symbol()
        {
            var result = await _navController.GetHistoricalUsdcNetAssetValues(InvalidIndiceOrCompositionSymbol,
                _startTime, Period.Hour,
                _endTime);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value.ToString().Should().Contain("not a valid symbol");
        }

        [Fact]
        public async Task GetHistoricalUsdcNetAssetValues_should_fail_on_unknown_composition_symbol()
        {
            var symbol = _mockCreator.GetRandomCompositionSymbol();
            _indiceProvider.GetCompositionFromSymbol(default).ReturnsForAnyArgs(default(IIndiceComposition));

            var result = await _navController.GetHistoricalUsdcNetAssetValues(symbol, _startTime,
                Period.Minute, _endTime);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value.ToString().Should()
                .Contain("Unknown composition").And
                .Contain(symbol);
        }

        [Fact]
        public async Task GetHistoricalUsdcNetAssetValues_should_fail_on_unknown_index_symbol()
        {
            var symbol = _mockCreator.GetRandomIndiceSymbol();
            _indiceProvider.GetDefinitionFromSymbol(default).ReturnsForAnyArgs(default(IIndiceDefinition));

            var result = await _navController.GetHistoricalUsdcNetAssetValues(symbol, _startTime, Period.Hour);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value.ToString().Should()
                .Contain("Unknown index").And
                .Contain(symbol);
        }

        [Fact]
        public async Task
            GetHistoricalUsdcNetAssetValues_from_composition_should_retrieve_composition_and_call_navCalculator()
        {
            var (composition, valuations) = GetFakeValuations();

            _indiceProvider.GetCompositionFromSymbol(composition.Symbol).Returns(composition);
            
            var daily = Period.Day;
            _navCalculator.GetCompositionValuations(composition, _startTime, daily, _endTime).Returns(valuations);

            var result = await _navController.GetHistoricalUsdcNetAssetValues(composition.Symbol, _startTime,
                daily, _endTime);

            await _indiceProvider.Received(1).GetCompositionFromSymbol(composition.Symbol);
            await _navCalculator.Received(1).GetCompositionValuations(composition, _startTime, daily, _endTime);
            await _navCalculator.DidNotReceiveWithAnyArgs().GetIndexValuations(default, default, default);

            CheckDetailsHistoricalNavs(result, daily, valuations, composition.Symbol);
        }

        [Fact]
        public async Task
            GetHistoricalUsdcNetAssetValues_from_index_definition_should_retrieve_index_definition_and_call_navCalculator()
        {
            var (composition, valuations) = GetFakeValuations();
            var definition = composition.IndiceDefinition;

            var hourly = Period.Hour;
            _navCalculator.GetIndexValuations(definition, _startTime, hourly, _endTime)
                .Returns(valuations);

            _indiceProvider.GetDefinitionFromSymbol(definition.Symbol)
                .Returns(definition);

            var result = await _navController.GetHistoricalUsdcNetAssetValues(definition.Symbol,
                _startTime, hourly, _endTime);

            await _indiceProvider.Received(1).GetDefinitionFromSymbol(definition.Symbol);
            await _navCalculator.Received(1)
                .GetIndexValuations(definition, _startTime, hourly, _endTime);
            await _navCalculator.DidNotReceiveWithAnyArgs().GetCompositionValuations(default, default, default);

            CheckDetailsHistoricalNavs(result, hourly, valuations, definition.Symbol);
        }

        private void CheckDetailsHistoricalNavs(ActionResult<DetailedHistoricalNavsByTimestampModel> result,
            Period period, List<IIndiceValuation> valuations, string expectedSymbol)
        {
            result.Result.Should().BeOfType<OkObjectResult>();
            var details = (DetailedHistoricalNavsByTimestampModel)((OkObjectResult)result.Result).Value;
            details.StartTime.Should().Be(_startTime);
            details.EndTime.Should().Be(_endTime);
            details.Period.Should().Be(period);
            details.ValuationsByTimeStamp.Count.Should().Be(valuations.Count);
            details.ValuationsByTimeStamp.Keys.Should().BeEquivalentTo(valuations.Select(v => v.TimeStamp));
            details.ValuationsByTimeStamp.Values.Should().BeEquivalentTo(valuations.Select(v => new IndiceValuationModel(v)));

            details.Symbol.Should().Be(expectedSymbol);
        }

        private (IIndiceComposition, List<IIndiceValuation>) GetFakeValuations()
        {
            var composition = _mockCreator.GetIndiceComposition(2);
            var valuations = Enumerable.Range(0, 5).Select(i => _mockCreator.GetRandomIndexValuation(composition))
                .ToList();
            return (composition, valuations);
        }
    }
}