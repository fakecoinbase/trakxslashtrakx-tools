using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Pricing;
using Trakx.MarketData.Server.Controllers;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Server.Controllers
{
    public class NavControllerTests
    {
        private readonly IIndiceDataProvider _indiceProvider;
        private readonly INavCalculator _navCalculator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly NavController _navController;
        private readonly MockCreator _mockCreator;

        public NavControllerTests(ITestOutputHelper output)
        {
            _indiceProvider = Substitute.For<IIndiceDataProvider>();
            _navCalculator = Substitute.For<INavCalculator>();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _navController = new NavController(_indiceProvider, _navCalculator, _dateTimeProvider, Substitute.For<ILogger<NavController>>());
            _mockCreator = new MockCreator(output);
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_look_for_indice_composition_at_date_if_given_indice_symbol()
        {
            var symbol = _mockCreator.GetRandomIndiceSymbol();
            var cancellationToken = new CancellationToken();

            _indiceProvider.GetCompositionAtDate(default, default, default).ReturnsForAnyArgs((IIndiceComposition)default);
            var result = await _navController.GetUsdNetAssetValue(symbol, cancellationToken: cancellationToken);

            await _indiceProvider.Received(1)
                .GetCompositionAtDate(Arg.Is(symbol), Arg.Any<DateTime>(), Arg.Is(cancellationToken));
            await _indiceProvider.DidNotReceiveWithAnyArgs().GetCompositionFromSymbol(default, default);

            ((JsonResult)result.Result).Value.Should().Be($"failed to retrieve composition for indice {symbol}.");
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_look_for_indice_composition_from_symbol_if_given_composition_symbol()
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
            var invalidSymbol = "abcdef";
            var cancellationToken = new CancellationToken();
            var result = await _navController.GetUsdNetAssetValue(invalidSymbol, cancellationToken: cancellationToken);

            await _indiceProvider.DidNotReceiveWithAnyArgs().GetCompositionAtDate(default, default, cancellationToken);
            await _navCalculator.DidNotReceiveWithAnyArgs().GetIndiceValuation(default, default);

            ((JsonResult)result.Result).Value.Should().Be($"{invalidSymbol} is not a valid symbol.");
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
    }
}