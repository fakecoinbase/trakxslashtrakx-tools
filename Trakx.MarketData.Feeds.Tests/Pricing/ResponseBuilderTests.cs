using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentAssertions;

using NSubstitute;
using NSubstitute.Exceptions;

using Trakx.MarketData.Feeds.Common.Pricing;
using Trakx.MarketData.Feeds.Common.Trackers;

using Xunit;

namespace Trakx.MarketData.Feeds.Tests.Pricing
{
    public class ResponseBuilderTests
    {
        [Fact]
        public void CalculatePriceMultiFullResponse_Should_Perform_Calculations()
        {
            var l2tbx = "L2TBX002";
            var i1tbe = "I1TBE002";
            var i3tex = "I3TEX002";
            var symbolsByTracker = new Dictionary<string, IList<string>>()
                                       {
                                           { l2tbx, new[] { "BTC", "XRP" } },
                                           { i1tbe, new[] { "BTC", "ETH" } },
                                           { i3tex, new[] { "ETH", "XRP" } },
                                       };

            var trackerFactory = Substitute.For<ITrackerFactory>();

            trackerFactory.FromTicker(l2tbx).Returns(new Tracker(2, "TBX", 2));
            trackerFactory.FromTicker(i1tbe).Returns(new Tracker(-1, "TBE", 2));
            trackerFactory.FromTicker(i3tex).Returns(new Tracker(-3, "TEX", 2));

            var pricer = new Pricer(trackerFactory);
            var responseBuilder = new ResponseBuilder(pricer, trackerFactory);

            var sampleResponse = TestData.CryptoCompare.PriceMultiFullResponse.Value;

            var result = responseBuilder.CalculatePriceMultiFullResponse(symbolsByTracker, sampleResponse);

            result.Raw.Keys.Count.Should().Be(3);
            result.Raw.Keys.Should().BeEquivalentTo(new[] { l2tbx, i1tbe, i3tex });

            result.Display.Keys.Count.Should().Be(3);
            result.Display.Keys.Should().BeEquivalentTo(new[] { l2tbx, i1tbe, i3tex });

            result.Raw.Values.Count.Should().Be(3);
            result.Raw.Values.All(v => v.Count == 2).Should().BeTrue();
            result.Raw.Values.All(v => v.ContainsKey("USD") && v.ContainsKey("EUR")).Should().BeTrue();

            result.Raw[i3tex]["EUR"].Price.Should().Be((double)(76.25m + 0.2538m) / 2d * -3);
            result.Raw[i1tbe]["USD"].ChangePCT24Hour.Should().Be((2.475754209468657m + 4.293595586471574m) / 2m * -1);
            result.Raw[l2tbx]["USD"].High24Hour.Should().Be((3318.31d + 0.2985d) / 2d * 2);
        }
    }
}
