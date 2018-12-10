using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;

using Trakx.MarketData.Feeds.Common.StaticData;

using Xunit;

namespace Trakx.MarketData.Feeds.Tests.StaticData
{
    public class TrakerSymbolsTests
    {
        [Fact]
        public void AllSymbols_Should_Return_All_Known_Symbols()
        {
            TrackerSymbols.AllSymbols.Should().BeEquivalentTo(new[] { "BTC", "ETH", "MC" });
        }

        [Fact]
        public void AllSingleNameSymbols_Should_Return_All_Known_Single_Name_Symbols()
        {
            TrackerSymbols.AllSingleNameSymbols.Should().BeEquivalentTo(new[] { "BTC", "ETH" });
        }

    }
}
