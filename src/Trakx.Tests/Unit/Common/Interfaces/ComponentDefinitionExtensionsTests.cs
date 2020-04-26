using FluentAssertions;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Indice;
using Trakx.Tests.Tools;
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
        [InlineData("wBTC", "USDc")]
        [InlineData("wETH", "wBTC")]
        public void GetLatestPriceCacheKey_should_lowercase_to_and_from_symbols(string fromSymbol, string toSymbol)
        {
            var expected = fromSymbol.ToNativeSymbol().ToLowerInvariant().GetLatestPriceCacheKey(toSymbol.ToNativeSymbol().ToLowerInvariant());
            fromSymbol.GetLatestPriceCacheKey(toSymbol).Should().Be(expected);
        }
    }
}