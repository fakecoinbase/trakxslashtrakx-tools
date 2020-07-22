using CryptoCompare;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Pricing;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Messari.Client;
using Trakx.Common.Utils;
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
        private readonly IIndiceComposition _composition;
        private readonly IIndiceDataProvider _indiceDataProvider;
        private readonly int _constituentCount;

        private static readonly Task<decimal?> FailedFetchPriceResult = Task.FromResult<decimal?>(default);


        public NavCalculatorTests(ITestOutputHelper output)
        {
            _messariClient = Substitute.For<IMessariClient>();
            _coinGeckoClient = Substitute.For<ICoinGeckoClient>();
            _cryptoCompareClient = Substitute.For<ICryptoCompareClient>();
            _cache = Substitute.For<IDistributedCache>();
            _mockCreator = new MockCreator(output);
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _dateTimeProvider.UtcNow.Returns(_mockCreator.GetRandomUtcDateTime());
            _indiceDataProvider = Substitute.For<IIndiceDataProvider>();
            _navCalculator = new NavCalculator(_messariClient, _coinGeckoClient, _cache, _cryptoCompareClient,
                    _indiceDataProvider, _dateTimeProvider, output.ToLogger<NavCalculator>());
            _constituentCount = 3;
            _composition = _mockCreator.GetIndiceComposition(_constituentCount);
        }

        [Theory]
        [InlineData(6, nameof(HistoryClient.MinutelyAsync))]
        [InlineData(7, nameof(HistoryClient.HourlyAsync))]
        [InlineData(365 * 3 - 1, nameof(HistoryClient.HourlyAsync))]
        [InlineData(366 * 3, nameof(HistoryClient.DailyAsync))]
        public async Task GetIndiceValuation_with_asOf_should_ask_CryptCompare_prices_with_adequate_granularity(int daysSinceAsOf, string expectedHistoryMethod)
        {
            var asOf = _dateTimeProvider.UtcNow.Subtract(TimeSpan.FromDays(daysSinceAsOf));

            MockCryptoCompareHistoricalResponses(asOf);

            var valuation = await _navCalculator.GetIndiceValuation(_composition, asOf);

            CryptoCompareShouldReceiveCallsFor(expectedHistoryMethod, _composition);

            _coinGeckoClient.ReceivedCalls().Should().BeEmpty();

            valuation.ComponentValuations.Count.Should().Be(_constituentCount);
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

        private HistoryResponse GetRandomCryptoCompareHistoryResponse(List<DateTime> expectedDateTimes = null)
        {
            expectedDateTimes ??= new List<DateTime> { _mockCreator.GetRandomUtcDateTime() };
            var candleData =
                expectedDateTimes.Select(d => new CandleData { Close = _mockCreator.GetRandomPrice(), Time = new DateTimeOffset(d) });
            var response = new HistoryResponse { Data = candleData.ToList().AsReadOnly() };
            return response;
        }

        [Fact]
        public async Task GetIndiceValuation_with_asOf_should_ask_CoingGecko_prices_second()
        {
            var asOf = _dateTimeProvider.UtcNow.AddDays(-5);

            _coinGeckoClient.GetPriceAsOfFromId(default, asOf)
                .ReturnsForAnyArgs(Task.FromResult((decimal?)101.23m));

            var valuation = await _navCalculator.GetIndiceValuation(_composition, asOf);

            await _cryptoCompareClient.History.ReceivedWithAnyArgs(3).MinutelyAsync(default, default);

            foreach (var coinGeckoId in _composition.GetComponentCoinGeckoIds())
            {
                await _coinGeckoClient.Received(1)
                    .GetPriceAsOfFromId(Arg.Is(coinGeckoId), Arg.Is(asOf));
            }

            valuation.ComponentValuations.Count.Should().Be(_constituentCount);
            valuation.NetAssetValue.Should().NotBe(0);
        }

        [Fact]
        public void GetIndiceValuation_with_asOf_should_not_ask_for_cached_or_messari_price()
        {
            new Func<Task>(async () =>
                await _navCalculator.GetIndiceValuation(_composition, _dateTimeProvider.UtcNow.AddMonths(-1)))
                .Should().Throw<NavCalculator.FailedToRetrievePriceException>();

            _messariClient.ReceivedCalls().Should().BeEmpty();
            _cache.ReceivedCalls().Should().BeEmpty();
        }

        [Fact]
        public void GetIndiceValuation_with_asOf_should_throw_on_future_asOf_date()
        {
            new Func<Task>(async () =>
                    await _navCalculator.GetIndiceValuation(_composition, _dateTimeProvider.UtcNow.AddSeconds(1)))
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
            _composition.ComponentQuantities[0].Quantity.Returns(12m);
            _composition.ComponentQuantities[1].Quantity.Returns(78.9m);
            _composition.ComponentQuantities[2].Quantity.Returns(17.92m);


            _messariClient.GetLatestPrice("sym0").Returns(21.2m);
            _messariClient.GetLatestPrice("sym1").Returns(FailedFetchPriceResult);
            _messariClient.GetLatestPrice("sym2").Returns(0.003m);

            _coinGeckoClient.GetLatestPrice("id-1").Returns(0.32m);

            var valuation = await _navCalculator.GetIndiceValuation(_composition);

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

        [Fact]
        public async Task GetCompositionValuations_should_only_return_results_for_timestamps_returned_for_all_components()
        {
            for (var i = 0; i < _constituentCount; i++)
            {
                var dates = Enumerable.Range(0, 4).Select(j => new DateTime(2019, 05, 02 + i + j)).ToList();
                var historicalResponse = GetRandomCryptoCompareHistoryResponse(dates);
                MockDailyHistoricalResponse(historicalResponse, _composition.ComponentQuantities[i].ComponentDefinition.Symbol);
            }

            var valuations = (await _navCalculator.GetCompositionValuations(_composition, _mockCreator.GetRandomUtcDateTime(),
                    Period.Day)).ToList();

            var expectedCommonDates = Enumerable.Range(0, _constituentCount)
                .Select(i => Enumerable.Range(0, 4).Select(j => new DateTime(2019, 05, 02 + i + j)))
                .IntersectMany()
                .ToList();

            valuations.Count.Should().Be(expectedCommonDates.Count);
            valuations.Select(v => v.TimeStamp).Should().BeEquivalentTo(expectedCommonDates);
        }


        [Fact]
        public void GetCompositionValuations_should_fail_if_one_component_has_no_price()
        {
            var failingSymbol = _composition.ComponentQuantities[1].ComponentDefinition.Symbol;
            var dates = Enumerable.Range(0, 2).Select(_ => _mockCreator.GetRandomUtcDateTime()).ToList();
            var historicalResponse = GetRandomCryptoCompareHistoryResponse(dates);

            MockDailyHistoricalResponse(historicalResponse, Arg.Is<string>(s => s != failingSymbol));

            _cryptoCompareClient.History.DailyAsync(Arg.Is<string>(s => s == failingSymbol),
                    Arg.Any<string>(), Arg.Any<int?>(), toDate: Arg.Any<DateTimeOffset>(), tryConversion: Arg.Any<bool>())
                .Throws(new TimeoutException("couldn't fetch prices'"));

            var getValuations = new Func<Task<IEnumerable<IIndiceValuation>>>(async () => await _navCalculator.GetCompositionValuations(_composition, _mockCreator.GetRandomUtcDateTime(),
                Period.Day));

            getValuations.Should()
                .Throw<NavCalculator.FailedToRetrievePriceException>("prices cannot be calculated without all prices.")
                .And.InnerException?.Message.Should().Contain(failingSymbol);
        }

        [Fact]
        public async Task GetCompositionValuations_should_fetch_daily_when_after_3_years()
        {
            var now = MockNow();

            var historicalResponse = GetRandomCryptoCompareHistoryResponse();
            MockDailyHistoricalResponse(historicalResponse, Arg.Any<string>());

            var moreThan3YearsAgo = now.Subtract(TimeSpan.FromDays(366 * 4));
            await _navCalculator.GetCompositionValuations(_composition, moreThan3YearsAgo, Period.Hour);

            await _cryptoCompareClient.History.ReceivedWithAnyArgs(_constituentCount)
                .DailyAsync(default, default, default);
        }

        [Fact]
        public async Task GetCompositionValuations_should_fetch_hourly_after_1_week()
        {
            var now = MockNow();

            var historicalResponse = GetRandomCryptoCompareHistoryResponse();
            MockHourlyHistoricalResponse(historicalResponse);

            var moreThan1WeekAgo = now.Subtract(TimeSpan.FromDays(8));
            await _navCalculator.GetCompositionValuations(_composition, moreThan1WeekAgo, Period.Minute);

            await _cryptoCompareClient.History.ReceivedWithAnyArgs(_constituentCount)
                .HourlyAsync(default, default);
        }

        [Fact]
        public async Task GetCompositionValuations_should_fetch_minutely_before_1_week()
        {
            var now = MockNow();

            _cryptoCompareClient.History.MinutelyAsync(Arg.Any<string>(),
                    Arg.Any<string>(), Arg.Any<int?>(), toDate: Arg.Any<DateTimeOffset>(), tryConversion: Arg.Any<bool>())
                .Returns(GetRandomCryptoCompareHistoryResponse());

            var moreThan1WeekAgo = now.Subtract(TimeSpan.FromDays(6));
            await _navCalculator.GetCompositionValuations(_composition, moreThan1WeekAgo, Period.Minute);

            await _cryptoCompareClient.History.ReceivedWithAnyArgs(_constituentCount)
                .MinutelyAsync(default, default);
        }

        private DateTime MockNow()
        {
            var now = _mockCreator.GetRandomUtcDateTime();
            _dateTimeProvider.UtcNow.Returns(now);
            return now;
        }

        [Fact]
        public async Task GetCompositionValuations_should_produce_valuations()
        {
            var now = MockNow();
            var startTime = now.Subtract(TimeSpan.FromHours(10));
            var endTime = startTime.AddHours(6);

            var expectedLimit = (int)endTime.Subtract(startTime).Divide(Period.Hour.ToTimeSpan());
            var expectedResultsTimeStamps = Enumerable.Range(0, expectedLimit).Select(i => startTime.AddHours(i)).ToList();

            var historicalResponse = GetRandomCryptoCompareHistoryResponse(expectedResultsTimeStamps);
            MockHourlyHistoricalResponse(historicalResponse, expectedLimit);

            var valuations = (await _navCalculator.GetCompositionValuations(_composition, startTime, Period.Hour, endTime))
                .ToList();

            valuations.Count.Should().Be(expectedLimit);
            valuations.Select(t => t.TimeStamp).Should().BeEquivalentTo(expectedResultsTimeStamps);
            valuations.SelectMany(v => v.ComponentValuations.Select(c => c.TimeStamp)).Distinct().Should()
                .BeEquivalentTo(expectedResultsTimeStamps);

            foreach (var valuation in valuations)
            {
                valuation.ComponentValuations.Count.Should().Be(_constituentCount);
                valuation.ComponentValuations.Select(c => c.TimeStamp).Distinct().Count().Should().Be(1);
                valuation.NetAssetValue.Should().Be(valuation.ComponentValuations.Sum(c => c.Value));
            }
        }

        [Fact]
        public async Task GetIndexValuations_should_get_compositions_from_indice_data_provider()
        {
            var now = MockNow();
            var indexDefinition = _mockCreator.GetRandomIndiceDefinition(creationDate: now.AddMonths(-4));
            var compositions = _mockCreator.GetIndiceCompositions(3, indexDefinition);
            var endTime = indexDefinition.CreationDate?.AddMonths(3);

            var compositionsByInterval = new Dictionary<TimeInterval, IIndiceComposition>
            {
                {new TimeInterval(compositions[0].CreationDate, compositions[1].CreationDate), compositions[0]},
                {new TimeInterval(compositions[1].CreationDate, compositions[2].CreationDate), compositions[1]},
                {new TimeInterval(compositions[2].CreationDate, endTime.Value), compositions[2]},
            };

            _indiceDataProvider.GetCompositionsBetweenDates(indexDefinition.Symbol,
                indexDefinition.CreationDate!.Value,
                endTime).Returns(compositionsByInterval);

            foreach (var composition in compositionsByInterval)
            {
                var expectedLimit = (int)composition.Key.EndTime.Subtract(composition.Key.StartTime).Divide(Period.Hour.ToTimeSpan()) + 1;
                var expectedResultsTimeStamps = Enumerable.Range(0, expectedLimit).Select(i => composition.Key.StartTime.AddDays(i)).ToList();
                var historicalResponse = GetRandomCryptoCompareHistoryResponse(expectedResultsTimeStamps);
                MockHourlyHistoricalResponse(historicalResponse);
            }

            await _navCalculator.GetIndexValuations(indexDefinition, compositions[0].CreationDate, Period.Hour, endTime);

            var receivedHourlyCalls = _cryptoCompareClient.History.ReceivedCalls()
                .Where(c => c.GetMethodInfo().Name == nameof(CryptoCompareClient.History.HourlyAsync)).ToList();
            receivedHourlyCalls.Count.Should().Be(compositions.Count * compositions.First().ComponentQuantities.Count);

            var expectedToDates = compositionsByInterval.Keys.Select(k => k.EndTime).ToList();
            foreach (var component in compositions.First().ComponentQuantities)
            {
                var receivedSymbolCalls = receivedHourlyCalls.Where(c => (string)c.GetArguments()[0] == component.ComponentDefinition.Symbol)
                    .ToList();
                receivedSymbolCalls.Count.Should().Be(compositionsByInterval.Count);

                receivedSymbolCalls.Select(c => ((DateTimeOffset?) c.GetArguments()[4]).Value.DateTime)
                    .Should().BeEquivalentTo(expectedToDates);
            }
        }
            


        private void MockHourlyHistoricalResponse(HistoryResponse historicalResponse, int? expectedLimit = default)
        {
            _cryptoCompareClient.History.HourlyAsync(Arg.Any<string>(),
                    Arg.Any<string>(), expectedLimit ?? Arg.Any<int?>(), toDate: Arg.Any<DateTimeOffset>(), tryConversion: Arg.Any<bool>())
                .Returns(historicalResponse);
        }

        private void MockDailyHistoricalResponse(HistoryResponse historicalResponse, string symbol)
        {
            _cryptoCompareClient.History.DailyAsync(symbol,
                    Arg.Any<string>(), Arg.Any<int?>(), toDate: Arg.Any<DateTimeOffset>(), tryConversion: Arg.Any<bool>())
                .Returns(historicalResponse);
        }

    }
}