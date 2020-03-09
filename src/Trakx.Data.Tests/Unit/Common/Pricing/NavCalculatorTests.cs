using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Data.Common.Core;
using Trakx.Data.Common.Pricing;
using Trakx.Data.Common.Sources.CoinGecko;
using Trakx.Data.Common.Sources.Messari.Client;
using Trakx.Data.Tests.Data;
using Xunit;

namespace Trakx.Data.Tests.Unit.Common.Pricing
{
    public class NavCalculatorTests
    {
        private readonly IMessariClient _messariClient;
        private readonly ICoinGeckoClient _coinGeckoClient;
        private readonly NavCalculator _navCalculator;
        private readonly MockCreator _mockCreator;

        public NavCalculatorTests()
        {
            _messariClient = Substitute.For<IMessariClient>();
            _coinGeckoClient = Substitute.For<ICoinGeckoClient>();
            _navCalculator = new NavCalculator(_messariClient, _coinGeckoClient, Substitute.For<ILogger<NavCalculator>>());
            _mockCreator = new MockCreator();

        }


        [Fact]
        public async Task GetIndexValuation_with_asOf_should_not_ask_for_messari_price()
        {
            var composition = _mockCreator.GetIndexComposition();
            var asOf = new DateTime(2020, 04, 13);
            _coinGeckoClient.GetPriceAsOf(default, asOf).ReturnsForAnyArgs(Task.FromResult((decimal?)101.23m));

            var valuation = await _navCalculator.GetIndexValuation(composition, asOf);

            _messariClient.ReceivedCalls().Should().BeEmpty();
            await _coinGeckoClient.Received(composition.ComponentQuantities.Count)
                .GetPriceAsOf(Arg.Any<string>(), Arg.Is(asOf));
        }
    }
}