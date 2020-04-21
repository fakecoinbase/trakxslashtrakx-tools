using FluentAssertions;
using Trakx.Common.Interfaces.Index;
using Xunit;

namespace Trakx.Tests.Unit.Common.Core
{
    public class SymbolExtensionsTests
    {
        [Fact]
        public void IsIndexSymbol_should_be_false_on_composition_symbols()
        {
            "l1len2003".IsIndexSymbol().Should().BeFalse();
            "s2amg2101".IsIndexSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsIndexSymbol_should_be_false_on_random_strings()
        {
            "abcds12kk".IsIndexSymbol().Should().BeFalse();
            "dk1asjj".IsIndexSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsIndexSymbol_should_be_true_on_index_symbols()
        {
            "l1len".IsIndexSymbol().Should().BeTrue();
            "s2amg".IsIndexSymbol().Should().BeTrue();
        }

        [Fact]
        public void IsCompositionSymbol_should_be_false_on_index_symbols()
        {
            "l1len".IsCompositionSymbol().Should().BeFalse();
            "s2amg".IsCompositionSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsCompositionSymbol_should_be_false_on_non_date_suffix_symbols()
        {
            "l1len2313".IsCompositionSymbol().Should().BeFalse();
            "s2amg2200".IsCompositionSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsCompositionSymbol_should_be_true_on_composition_symbols()
        {
            "l1len2006".IsCompositionSymbol().Should().BeTrue();
            "s2amg2308".IsCompositionSymbol().Should().BeTrue();
            "s2amg2312".IsCompositionSymbol().Should().BeTrue();
            "l9ibi3310".IsCompositionSymbol().Should().BeTrue();
        }

        [Fact]
        public void IsCompositionSymbol_should_not_match_on_Top10Erc20ByMarketCap_index_symbol()
        {
            "l1mcap10erc20".IsIndexSymbol().Should().BeTrue();
            "l1mcap10erc20".IsCompositionSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsIndexSymbol_should_not_match_on_Top10Erc20ByMarketCap_composition_symbol()
        {
            "l1mcap10erc202412".IsCompositionSymbol().Should().BeTrue();
            "l1mcap10erc202412".IsIndexSymbol().Should().BeFalse();
        }
    }
}
