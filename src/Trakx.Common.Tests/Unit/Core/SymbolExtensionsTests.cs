using FluentAssertions;
using Trakx.Common.Interfaces.Indice;
using Xunit;

namespace Trakx.Common.Tests.Unit.Core
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
        public void IsIndiceSymbol_should_be_false_if_less_than_4_chars_or_more_than_10()
        {
            "l1t".IsIndiceSymbol().Should().BeFalse();
            "s2b3".IsIndiceSymbol().Should().BeTrue();
            "s2abcdefgh".IsIndiceSymbol().Should().BeTrue();
            "s2abcdefghi".IsIndiceSymbol().Should().BeFalse();
        }

        [Fact]
        public void IsCompositionSymbol_should_be_false_if_less_than_8_chars_or_more_than_14()
        {
            "l1t2205".IsCompositionSymbol().Should().BeFalse();
            "s2b33612".IsCompositionSymbol().Should().BeTrue();
            "s2abcdefgh2109".IsCompositionSymbol().Should().BeTrue();
            "s2abcdefghi2606".IsCompositionSymbol().Should().BeFalse();
        }

        [Theory]
        [InlineData("l1amg")]
        [InlineData("l1cex")]
        [InlineData("l1dex")]
        [InlineData("l1len")]
        [InlineData("l1sca")]
        [InlineData("l1mc10erc")]
        [InlineData("l1btceth")]
        [InlineData("l1vol15btc")]
        [InlineData("l1vol20be")]
        public void IsCompositionSymbol_should_not_match_on_known_indice_symbols(string indiceSymbol)
        {
            indiceSymbol.IsIndiceSymbol().Should().BeTrue();
            indiceSymbol.IsCompositionSymbol().Should().BeFalse();
        }

        [Theory]
        [InlineData("l1amg2003")]
        [InlineData("l1cex2001")]
        [InlineData("l1dex2412")]
        [InlineData("l1len2311")]
        [InlineData("l1sca3005")]
        [InlineData("l1mc10erc2004")]
        [InlineData("l1btceth3311")]
        [InlineData("l1vol15btc2708")]
        [InlineData("l1vol20be2602")]
        public void IsIndiceSymbol_should_not_match_on_known_composition_symbol(string compositionSymbol)
        {
            compositionSymbol.IsCompositionSymbol().Should().BeTrue();
            compositionSymbol.IsIndiceSymbol().Should().BeFalse();
        }
    }
}
