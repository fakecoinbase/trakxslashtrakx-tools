using FluentAssertions;

using Trakx.MarketData.Feeds.Common.StaticData;

using Xunit;

namespace Trakx.MarketData.Feeds.Tests.StaticData
{
    public class TrackerDetailsTests
    {
        [Fact]
        public void GetAllCoins()
        {
            var trakxCoins = TrackerDetails.TrakxTrackersAsCoinList;
            trakxCoins.Coins.Count.Should().Be(18);
            trakxCoins.Coins.Should().OnlyHaveUniqueItems();
            trakxCoins.Coins["L2BTC"].CoinName.Should().Be("Long BTC Levered x2");
        }
    }
}
