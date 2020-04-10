using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Trakx.Common.Interfaces.Index;
using Trakx.Common.Pricing;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Messari.Client;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Common.Pricing
{
    public class NavCalculatorTests
    {
        private readonly IMessariClient _messariClient;
        private readonly ICoinGeckoClient _coinGeckoClient;
        private readonly NavCalculator _navCalculator;
        private readonly MockCreator _mockCreator;
        private static readonly Task<decimal?> FailedFetchPriceResult = Task.FromResult<decimal?>(default);

        public NavCalculatorTests(ITestOutputHelper output)
        {
            _messariClient = Substitute.For<IMessariClient>();
            _coinGeckoClient = Substitute.For<ICoinGeckoClient>();
            var cache = Substitute.For<IDistributedCache>();
            _navCalculator =
                new NavCalculator(_messariClient, _coinGeckoClient, cache, output.ToLogger<NavCalculator>());
            _mockCreator = new MockCreator();
        }


        [Fact]
        public async Task GetIndexValuation_with_asOf_should_not_ask_for_messari_price()
        {
            var composition = _mockCreator.GetIndexComposition(3);
            var asOf = new DateTime(2020, 04, 13);
            _coinGeckoClient.GetPriceAsOfFromId(default, asOf)
                .ReturnsForAnyArgs(Task.FromResult((decimal?) 101.23m));

            var valuation = await _navCalculator.GetIndexValuation(composition, asOf);

            _messariClient.ReceivedCalls().Should().BeEmpty();

            foreach (var coinGeckoId in composition.GetComponentCoinGeckoIds())
            {
                await _coinGeckoClient.Received(1)
                    .GetPriceAsOfFromId(Arg.Is(coinGeckoId), Arg.Is(asOf));
            }
            
            valuation.ComponentValuations.Count.Should().Be(composition.ComponentQuantities.Count);
            valuation.NetAssetValue.Should().NotBe(0);
        }

        [Fact]
        public async Task GetIndexValuation_without_asOf_should_only_ask_messari_price()
        {
            var composition = _mockCreator.GetIndexComposition(5);

            _messariClient.GetLatestPrice(default).ReturnsForAnyArgs(Task.FromResult((decimal?) 110.0));

            var valuation = await _navCalculator.GetIndexValuation(composition);

            foreach (var symbol in composition.GetComponentSymbols())
            {
                await _messariClient.Received(1)
                    .GetLatestPrice(Arg.Is(symbol));
            }

            _coinGeckoClient.ReceivedCalls().Should().BeEmpty();

            valuation.ComponentValuations.Count.Should().Be(composition.ComponentQuantities.Count);
            valuation.NetAssetValue.Should().NotBe(0);
        }

        [Fact]
        public async Task GetIndexValuation_without_asOf_should_fallback_on_coingecko_when_messari_price_is_unavailable()
        {
            var composition = _mockCreator.GetIndexComposition(4);
            var failSymbol = composition.ComponentQuantities[3].ComponentDefinition.Symbol;
            var failCoinGeckoId = composition.ComponentQuantities[3].ComponentDefinition.CoinGeckoId;

            _messariClient.GetLatestPrice(default)
                .ReturnsForAnyArgs(Task.FromResult((decimal?)110.0));
            _messariClient.GetLatestPrice(failSymbol).Returns(FailedFetchPriceResult);

            _coinGeckoClient.GetLatestPrice(failCoinGeckoId).Returns(123.45m);

            var valuation = await _navCalculator.GetIndexValuation(composition);

            foreach (var symbol in composition.GetComponentSymbols())
            {
                await _messariClient.Received(1)
                    .GetLatestPrice(Arg.Is(symbol));
            }

            _coinGeckoClient.ReceivedCalls().Count().Should().Be(1);
            await _coinGeckoClient.Received(1).GetLatestPrice(Arg.Is(failCoinGeckoId));

            valuation.ComponentValuations.Count.Should().Be(composition.ComponentQuantities.Count);
            valuation.NetAssetValue.Should().NotBe(0);
        }
        
        [Fact]
        public void GetIndexValuation_without_asOf_should_fail_when_no_price_found_for_1_component()
        {
            var composition = _mockCreator.GetIndexComposition(4);
            var failSymbol = composition.ComponentQuantities[3].ComponentDefinition.Symbol;
            var failCoinGeckoId = composition.ComponentQuantities[3].ComponentDefinition.CoinGeckoId;

            _messariClient.GetLatestPrice(default)
                .ReturnsForAnyArgs(Task.FromResult((decimal?)110.0));
            _messariClient.GetLatestPrice(failSymbol).Returns(FailedFetchPriceResult);

            _coinGeckoClient.GetLatestPrice(failCoinGeckoId).Returns(FailedFetchPriceResult);

            new Func<Task>(async () => await _navCalculator.GetIndexValuation(composition))
                .Should().ThrowExactly<NavCalculator.FailedToRetrievePriceException>();
        }

        [Fact]
        public async Task GetIndexValuation_should_produce_component_valuations_and_correct_prices_and_weights()
        {
            var composition = _mockCreator.GetIndexComposition(3);
            composition.ComponentQuantities[0].Quantity.Returns(12m);
            composition.ComponentQuantities[1].Quantity.Returns(78.9m);
            composition.ComponentQuantities[2].Quantity.Returns(17.92m);
            
                
            _messariClient.GetLatestPrice("sym0").Returns(21.2m);
            _messariClient.GetLatestPrice("sym1").Returns(FailedFetchPriceResult);
            _messariClient.GetLatestPrice("sym2").Returns(0.003m);

            _coinGeckoClient.GetLatestPrice("id-1").Returns(0.32m);

            var utcNow = DateTime.UtcNow;
            var valuation = await _navCalculator.GetIndexValuation(composition);

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
            valuation.TimeStamp.Subtract(utcNow)
                .Should().BeCloseTo(TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

        }
    }
}