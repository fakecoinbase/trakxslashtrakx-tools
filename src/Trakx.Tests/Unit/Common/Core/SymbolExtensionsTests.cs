using FluentAssertions;
using Trakx.Common.Interfaces.Indice;
using Xunit;

namespace Trakx.Tests.Unit.Common.Core
{
    public class SymbolExtensionsTests
    {
        [Fact]
        public void IsIndiceSymbol_should_be_false_on_composition_symbols()
        {
            "l1len2003".IsIndiceSymbol().Should().BeFalse();
            "s2amg2101".IsIndiceSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsIndiceSymbol_should_be_false_on_random_strings()
        {
            "abcds12kk".IsIndiceSymbol().Should().BeFalse();
            "dk1asjj".IsIndiceSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsIndiceSymbol_should_be_true_on_indice_symbols()
        {
            "l1len".IsIndiceSymbol().Should().BeTrue();
            "s2amg".IsIndiceSymbol().Should().BeTrue();
        }

        [Fact]
        public void IsCompositionSymbol_should_be_false_on_indice_symbols()
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
        public void IsCompositionSymbol_should_not_match_on_Top10Erc20ByMarketCap_indice_symbol()
        {
            "l1mcap10erc20".IsIndiceSymbol().Should().BeTrue();
            "l1mcap10erc20".IsCompositionSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsIndiceSymbol_should_not_match_on_Top10Erc20ByMarketCap_composition_symbol()
        {
            "l1mcap10erc202412".IsCompositionSymbol().Should().BeTrue();
            "l1mcap10erc202412".IsIndiceSymbol().Should().BeFalse();
        }
    }
}
