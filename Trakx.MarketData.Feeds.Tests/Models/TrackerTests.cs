using System;
using System.Linq;

using FluentAssertions;

using Trakx.MarketData.Feeds.Common.Models.Trakx;
using Trakx.MarketData.Feeds.Common.Trackers;

using Xunit;

namespace Trakx.MarketData.Feeds.Tests.Models
{
    public class TrackerTests
    {
        [Fact]
        public void ConstructorFromString_Should_Extract_Leverage_Direction()
        {
            var validTickers = new[] { "L2BTC", "I3ETH", "L1MC005", "I2MC020" };
            var expectedLeverages = new[] { 2, -3, 1, -2 };

            var interpreted = validTickers.Select(t => new Tracker(t).Leverage).ToList();

            interpreted.Should().BeEquivalentTo(expectedLeverages);
        }

        [Fact]
        public void BasketSize_When_Size_Is_Expressed_Should_Return_Count()
        {
            var tracker = new Tracker("L1MC004");
            tracker.BasketSize.Should().Be(4);
        }

        [Fact]
        public void BasketSize_When_Count_Is_Not_Expressed_Should_Return_0()
        {
            var tracker = new Tracker("I1BTC");
            tracker.BasketSize.Should().Be(1);
        }

        [Fact]
        public void ConstructorFromString_Should_Fail_On_Unknown_Ticker()
        {
            new Action(() => new Tracker("I2FIU007")).Should().Throw<Exception>();
        }
    }
}