using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CryptoCompare;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Pricing;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Messari.Client;
using Trakx.Common.Utils;
using Trakx.Persistence.Tests;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Common.Tests.Unit.Pricing
{
    public class NavCalculatorTests
    {
        private readonly IMessariClient _messariClient;
        private readonly ICoinGeckoClient _coinGeckoClient;
        private readonly NavCalculator _navCalculator;
        private readonly MockCreator _mockCreator;
        private readonly ICryptoCompareClient _cryptoCompareClient;
        private readonly IDistributedCache _cache;
        private readonly IDateTimeProvider _dateTimeProvider;
        private static readonly Task<decimal?> FailedFetchPriceResult = Task.FromResult<decimal?>(default);

        public NavCalculatorTests(ITestOutputHelper output)
        {
            _messariClient = Substitute.For<IMessariClient>();
            _coinGeckoClient = Substitute.For<ICoinGeckoClient>();
            _cryptoCompareClient = Substitute.For<ICryptoCompareClient>();
            _cache = Substitute.For<IDistributedCache>();
            _mockCreator = new MockCreator(output);
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _dateTimeProvider.UtcNow.Returns(_mockCreator.GetRandomDateTime());
            _navCalculator =
                new NavCalculator(_messariClient, _coinGeckoClient, _cache, _cryptoCompareClient,
                    _dateTimeProvider, output.ToLogger<NavCalculator>());
        }

        [Theory]
        [InlineData(6, nameof(HistoryClient.MinutelyAsync))]
        [InlineData(7, nameof(HistoryClient.HourlyAsync))]
        [InlineData(365 * 3 - 1, nameof(HistoryClient.HourlyAsync))]
        [InlineData(366 * 3, nameof(HistoryClient.DailyAsync))]
        public async Task GetIndiceValuation_with_asOf_should_ask_CryptCompare_prices_with_adequate_granularity(int daysSinceAsOf, string expectedHistoryMethod)
        {
            var composition = _mockCreator.GetIndiceComposition(3);
            var asOf = _dateTimeProvider.UtcNow.Subtract(TimeSpan.FromDays(daysSinceAsOf));

            MockCryptoCompareHistoricalResponses(asOf);

            var valuation = await _navCalculator.GetIndiceValuation(composition, asOf);

            CryptoCompareShouldReceiveCallsFor(expectedHistoryMethod, composition);

            _coinGeckoClient.ReceivedCalls().Should().BeEmpty();

            valuation.ComponentValuations.Count.Should().Be(composition.ComponentQuantities.Count);
            valuation.NetAssetValue.Should().NotBe(0);
        }

        private void MockCryptoCompareHistoricalResponses(DateTime asOf)
        {
            _cryptoCompareClient.History.MinutelyAsync(Arg.Any<string>(), "usdc", 1, toDate: asOf, tryConversion: true)
                .Returns(GetRandomCryptoCompareHistoryResponse());
            _cryptoCompareClient.History.HourlyAsync(Arg.Any<string>(), "usdc", 1, toDate: asOf, tryConversion: true)
                .Returns(GetRandomCryptoCompareHistoryResponse());
            _cryptoCompareClient.History.DailyAsync(Arg.Any<string>(), "usdc", 1, toDate: asOf, tryConversion: true)
                .Returns(GetRandomCryptoCompareHistoryResponse());
        }

        private void CryptoCompareShouldReceiveCallsFor(string expectedHistoryMethod, IIndiceComposition composition)
        {
            var receivedCalls = _cryptoCompareClient.History.ReceivedCalls().ToList();
            receivedCalls.Count.Should().Be(3);
            receivedCalls.Select(c => c.GetMethodInfo().Name).Distinct()
                .Should().BeEquivalentTo(expectedHistoryMethod);
            receivedCalls.Select(c => (string)c.GetArguments().First()).Should()
                .BeEquivalentTo(composition.ComponentQuantities.Select(q => q.ComponentDefinition.Symbol));
        }

        private HistoryResponse GetRandomCryptoCompareHistoryResponse()
        {
            var candleData = new List<CandleData> { new CandleData { Close = _mockCreator.GetRandomPrice() } };
            var response = new HistoryResponse { Data = candleData };
            return response;
        }

        [Fact]
        public async Task GetIndiceValuation_with_asOf_should_ask_CoingGecko_prices_second()
        {
            var composition = _mockCreator.GetIndiceComposition(3);
            var asOf = _dateTimeProvider.UtcNow.AddDays(-5);

            _coinGeckoClient.GetPriceAsOfFromId(default, asOf)
                .ReturnsForAnyArgs(Task.FromResult((decimal?)101.23m));

            var valuation = await _navCalculator.GetIndiceValuation(composition, asOf);

            await _cryptoCompareClient.History.ReceivedWithAnyArgs(3).MinutelyAsync(default, default);

            foreach (var coinGeckoId in composition.GetComponentCoinGeckoIds())
            {
                await _coinGeckoClient.Received(1)
                    .GetPriceAsOfFromId(Arg.Is(coinGeckoId), Arg.Is(asOf));
            }

            valuation.ComponentValuations.Count.Should().Be(composition.ComponentQuantities.Count);
            valuation.NetAssetValue.Should().NotBe(0);
        }

        [Fact]
        public async Task GetIndiceValuation_with_asOf_should_not_ask_for_cached_or_messari_price()
        {
            var composition = _mockCreator.GetIndiceComposition(3);

            new Func<Task>(async () =>
                await _navCalculator.GetIndiceValuation(composition, _dateTimeProvider.UtcNow.AddMonths(-1)))
                .Should().Throw<NavCalculator.FailedToRetrievePriceException>();

            _messariClient.ReceivedCalls().Should().BeEmpty();
            _cache.ReceivedCalls().Should().BeEmpty();
        }

        [Fact]
        public void GetIndiceValuation_with_asOf_should_throw_on_future_asOf_date()
        {
            var composition = _mockCreator.GetIndiceComposition(3);

            new Func<Task>(async () =>
                    await _navCalculator.GetIndiceValuation(composition, _dateTimeProvider.UtcNow.AddSeconds(1)))
                .Should().Throw<ArgumentOutOfRangeException>();

            _messariClient.ReceivedCalls().Should().BeEmpty();
            _cache.ReceivedCalls().Should().BeEmpty();
            _coinGeckoClient.ReceivedCalls().Should().BeEmpty();
            _cryptoCompareClient.History.ReceivedCalls().Should().BeEmpty();
        }

        [Fact]
        public async Task GetIndiceValuation_without_asOf_should_look_in_cache_first()
        {
            var composition = _mockCreator.GetIndiceComposition(5);

            _cache.GetAsync(default).ReturnsForAnyArgs(Task.FromResult(110.0m.GetBytes()));

            var valuation = await _navCalculator.GetIndiceValuation(composition);

            await CacheShouldReceiveCallsForComponentPrices(composition).ConfigureAwait(false);

            _cryptoCompareClient.ReceivedCalls().Should().BeEmpty();
            _messariClient.ReceivedCalls().Should().BeEmpty();
            _coinGeckoClient.ReceivedCalls().Should().BeEmpty();

            ValuationShouldBeValid(valuation, composition);
        }

        [Fact]
        public async Task GetIndiceValuation_without_asOf_should_fetch_prices_from_messari_second()
        {
            var composition = _mockCreator.GetIndiceComposition(5);

            _messariClient.GetLatestPrice(default).ReturnsForAnyArgs(Task.FromResult((decimal?)110.0));

            var valuation = await _navCalculator.GetIndiceValuation(composition);

            await CacheShouldReceiveCallsForComponentPrices(composition).ConfigureAwait(false);
            await MessariShouldReceiveCallsForComponentPrices(composition).ConfigureAwait(false);

            _coinGeckoClient.ReceivedCalls().Should().BeEmpty();

            ValuationShouldBeValid(valuation, composition);
        }

        [Fact]
        public async Task GetIndiceValuation_without_asOf_should_fallback_on_coingecko_when_messari_price_is_unavailable()
        {
            var composition = _mockCreator.GetIndiceComposition(4);
            var failSymbol = composition.ComponentQuantities[3].ComponentDefinition.Symbol;
            var failCoinGeckoId = composition.ComponentQuantities[3].ComponentDefinition.CoinGeckoId;

            _messariClient.GetLatestPrice(default)
                .ReturnsForAnyArgs(Task.FromResult((decimal?)110.0));
            _messariClient.GetLatestPrice(failSymbol).Returns(FailedFetchPriceResult);

            _coinGeckoClient.GetLatestPrice(failCoinGeckoId).Returns(123.45m);

            var valuation = await _navCalculator.GetIndiceValuation(composition);

            await CacheShouldReceiveCallsForComponentPrices(composition).ConfigureAwait(false);
            await MessariShouldReceiveCallsForComponentPrices(composition).ConfigureAwait(false);

            _coinGeckoClient.ReceivedCalls().Count().Should().Be(1);
            await _coinGeckoClient.Received(1).GetLatestPrice(Arg.Is(failCoinGeckoId))
                .ConfigureAwait(false);

            ValuationShouldBeValid(valuation, composition);
        }

        [Fact]
        public void GetIndiceValuation_without_asOf_should_fail_when_no_price_found_for_1_component()
        {
            var composition = _mockCreator.GetIndiceComposition(4);
            var failSymbol = composition.ComponentQuantities[3].ComponentDefinition.Symbol;
            var failCoinGeckoId = composition.ComponentQuantities[3].ComponentDefinition.CoinGeckoId;

            _messariClient.GetLatestPrice(default)
                .ReturnsForAnyArgs(Task.FromResult((decimal?)110.0));
            _messariClient.GetLatestPrice(failSymbol).Returns(FailedFetchPriceResult);

            _coinGeckoClient.GetLatestPrice(failCoinGeckoId).Returns(FailedFetchPriceResult);

            new Func<Task>(async () => await _navCalculator.GetIndiceValuation(composition))
                .Should().ThrowExactly<NavCalculator.FailedToRetrievePriceException>();
        }

        private static void ValuationShouldBeValid(IIndiceValuation valuation, IIndiceComposition composition)
        {
            valuation.ComponentValuations.Count.Should().Be(composition.ComponentQuantities.Count);
            valuation.NetAssetValue.Should().NotBe(0);
        }

        private async Task CacheShouldReceiveCallsForComponentPrices(IIndiceComposition composition)
        {
            foreach (var symbol in composition.GetComponentSymbols())
            {
                await _cache.Received(1).GetAsync(Arg.Is(symbol.GetLatestPriceCacheKey("usdc")));
            }
        }

        private async Task MessariShouldReceiveCallsForComponentPrices(IIndiceComposition composition)
        {
            foreach (var symbol in composition.GetComponentSymbols())
            {
                await _messariClient.Received(1).GetLatestPrice(Arg.Is(symbol));
            }
        }

        [Fact]
        public async Task GetIndiceValuation_should_produce_component_valuations_and_correct_prices_and_weights()
        {
            var composition = _mockCreator.GetIndiceComposition(3);
            composition.ComponentQuantities[0].Quantity.Returns(12m);
            composition.ComponentQuantities[1].Quantity.Returns(78.9m);
            composition.ComponentQuantities[2].Quantity.Returns(17.92m);


            _messariClient.GetLatestPrice("sym0").Returns(21.2m);
            _messariClient.GetLatestPrice("sym1").Returns(FailedFetchPriceResult);
            _messariClient.GetLatestPrice("sym2").Returns(0.003m);

            _coinGeckoClient.GetLatestPrice("id-1").Returns(0.32m);

            var valuation = await _navCalculator.GetIndiceValuation(composition);

            valuation.ComponentValuations[0].Price.Should().Be(21.2m);
            valuation.ComponentValuations[0].Value.Should().Be(12 * 21.2m);
            valuation.ComponentValuations[0].PriceSource.Should().Be("messari");

            valuation.ComponentValuations[1].Price.Should().Be(0.32m);
            valuation.ComponentValuations[1].Value.Should().Be(78.9m * 0.32m);
            valuation.ComponentValuations[1].PriceSource.Should().Be("coinGecko");

            valuation.ComponentValuations[2].Price.Should().Be(0.003m);
            valuation.ComponentValuations[2].Value.Should().Be(17.92m * 0.003m);
            valuation.ComponentValuations[2].PriceSource.Should().Be("messari");

            valuation.ComponentValuations.Sum(c => c.Weight).Should().BeApproximately(1, 1e-3);

            var summedValues = valuation.ComponentValuations.Sum(c => c.Value);
            valuation.NetAssetValue.Should().Be(summedValues);
            valuation.TimeStamp.Should().Be(_dateTimeProvider.UtcNow);
        }

        [Fact]
        public async Task GetIndiceValuation_should_use_native_token_symbols()
        {
            var composition = _mockCreator.GetIndiceComposition(2);
            var wrappedBitcoinSymbol = "wbtc";
            var wrappedEtherSymbol = "weth";
            composition.ComponentQuantities[0].ComponentDefinition.Symbol.Returns(wrappedBitcoinSymbol);
            composition.ComponentQuantities[1].ComponentDefinition.Symbol.Returns(wrappedEtherSymbol);

            _coinGeckoClient.GetLatestPrice("id-0").Returns(12.3m);
            _coinGeckoClient.GetLatestPrice("id-1").Returns(45.6m);

            var valuation = await _navCalculator.GetIndiceValuation(composition);

            await _cache.Received(1).GetAsync(Arg.Is(wrappedBitcoinSymbol.GetLatestPriceCacheKey("usdc")));
            await _cache.Received(1).GetAsync(Arg.Is(wrappedEtherSymbol.GetLatestPriceCacheKey("usdc")));

            await _messariClient.Received(1).GetLatestPrice(Arg.Is(wrappedBitcoinSymbol.ToNativeSymbol()));
            await _messariClient.Received(1).GetLatestPrice(Arg.Is(wrappedEtherSymbol.ToNativeSymbol()));

            valuation.ComponentValuations.Select(v => v.ComponentQuantity.ComponentDefinition.Symbol)
                .Should().BeEquivalentTo(wrappedBitcoinSymbol, wrappedEtherSymbol);
        }
    }
}