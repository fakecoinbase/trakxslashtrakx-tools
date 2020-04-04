using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Index;
using Trakx.Common.Interfaces.Pricing;
using Trakx.MarketData.Server.Controllers;
using Trakx.Tests.Data;
using Xunit;

namespace Trakx.Tests.Unit.Server.Controllers
{
    public class NavControllerTests
    {
        private readonly IIndexDataProvider _indexProvider;
        private readonly INavCalculator _navCalculator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly NavController _navController;
        private readonly MockCreator _mockCreator;

        public NavControllerTests()
        {
            _indexProvider = Substitute.For<IIndexDataProvider>();
            _navCalculator = Substitute.For<INavCalculator>();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _navController = new NavController(_indexProvider, _navCalculator, _dateTimeProvider, Substitute.For<ILogger<NavController>>());
            _mockCreator = new MockCreator();
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_look_for_index_composition_at_date_if_given_index_symbol()
        {
            var symbol = _mockCreator.GetRandomIndexSymbol();
            var cancellationToken = new CancellationToken();

            _indexProvider.GetCompositionAtDate(default, default, default).ReturnsForAnyArgs((IIndexComposition)default);
            var result = await _navController.GetUsdNetAssetValue(symbol, cancellationToken: cancellationToken);

            await _indexProvider.Received(1)
                .GetCompositionAtDate(Arg.Is(symbol), Arg.Any<DateTime>(), Arg.Is(cancellationToken));
            await _indexProvider.DidNotReceiveWithAnyArgs().GetCompositionFromSymbol(default, default);

            ((JsonResult)result.Result).Value.Should().Be($"failed to retrieve composition for index {symbol}.");
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_look_for_index_composition_from_symbol_if_given_composition_symbol()
        {
            var symbol = _mockCreator.GetRandomCompositionSymbol();
            var cancellationToken = new CancellationToken();
            var result = await _navController.GetUsdNetAssetValue(symbol, cancellationToken: cancellationToken);

            await _indexProvider.DidNotReceiveWithAnyArgs().GetCompositionAtDate(default, default, cancellationToken);
            await _indexProvider.Received(1).GetCompositionFromSymbol(Arg.Is(symbol), Arg.Is(cancellationToken));
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_return_immediately_on_bad_symbols()
        {
            var invalidSymbol = "abcdef";
            var cancellationToken = new CancellationToken();
            var result = await _navController.GetUsdNetAssetValue(invalidSymbol, cancellationToken: cancellationToken);

            await _indexProvider.DidNotReceiveWithAnyArgs().GetCompositionAtDate(default, default, cancellationToken);
            await _navCalculator.DidNotReceiveWithAnyArgs().GetIndexValuation(default, default);

            ((JsonResult)result.Result).Value.Should().Be($"{invalidSymbol} is not a valid symbol.");
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_retrieve_composition_asOf_utcNow_if_compositionAsOf_unspecified()
        {
            var cancellationToken = new CancellationToken();
            var utcNow = new DateTime(2020, 02, 29);
            var priceAsOf = new DateTime(2020, 02, 10);
            _dateTimeProvider.UtcNow.Returns(utcNow);

            var composition = _mockCreator.GetIndexComposition(2);
            _indexProvider.GetCompositionAtDate(Arg.Any<string>(), utcNow, cancellationToken)
                .Returns(composition);

            var mockValuation = Substitute.For<IIndexValuation>();
            var nav = 123.456m;
            mockValuation.NetAssetValue.Returns(nav);
            _navCalculator.GetIndexValuation(composition, priceAsOf).Returns(mockValuation);

            var indexSymbol = _mockCreator.GetRandomIndexSymbol();

            var result = await _navController.GetUsdNetAssetValue(indexSymbol,
                priceAsOf, default, 0, cancellationToken);

            await _indexProvider.Received(1).GetCompositionAtDate(indexSymbol, utcNow, cancellationToken);
            await _navCalculator.Received(1).GetIndexValuation(composition, priceAsOf);

            ((JsonResult)result.Result).Value.Should().Be(nav);
        }

        [Fact]
        public async Task GetUsdNetAssetValue_should_retrieve_prices_asOf_utcNow_if_pricesAsOf_unspecified()
        {
            var cancellationToken = new CancellationToken();
            var valuationAsOf = default(DateTime?);
            var compositionAsOf = new DateTime(2020, 03, 01);

            var composition = _mockCreator.GetIndexComposition(3);
            _indexProvider.GetCompositionAtDate(Arg.Any<string>(), compositionAsOf, cancellationToken)
                .Returns(composition);

            var mockValuation = Substitute.For<IIndexValuation>();
            var nav = 78.9m;
            mockValuation.NetAssetValue.Returns(nav);
            _navCalculator.GetIndexValuation(composition, valuationAsOf).Returns(mockValuation);

            var indexSymbol = _mockCreator.GetRandomIndexSymbol();

            var result = await _navController.GetUsdNetAssetValue(indexSymbol,
                default, compositionAsOf, 0, cancellationToken);

            await _indexProvider.Received(1).GetCompositionAtDate(indexSymbol, compositionAsOf, cancellationToken);
            await _navCalculator.Received(1).GetIndexValuation(composition, default);

            ((JsonResult)result.Result).Value.Should().Be(nav);
        }
    }
}