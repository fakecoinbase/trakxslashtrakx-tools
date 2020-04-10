using System.Globalization;
using FluentAssertions;
using NSubstitute;
using Trakx.Common.Interfaces.Index;
using Xunit;

namespace Trakx.Tests.Unit.Common.Interfaces
{
    public class ComponentDefinitionExtensionsTests
    {
        [Theory]
        [InlineData("Usd", "Btc")]
        [InlineData("GBP", "EUR")]
        [InlineData("eth", "LTC")]
        [InlineData("XRP", "tBtc")]
        public void GetLatestPriceCacheKey_should_lowercase_to_and_from_symbols(string fromSymbol, string toSymbol)
        {
            var expected = fromSymbol.ToLowerInvariant().GetLatestPriceCacheKey(toSymbol.ToLowerInvariant());
            fromSymbol.GetLatestPriceCacheKey(toSymbol).Should().Be(expected);
        }
    }
}