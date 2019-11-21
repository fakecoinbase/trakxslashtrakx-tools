using FluentAssertions;
using Microsoft.Extensions.ObjectPool;
using Trakx.Data.Market.Common.Sources.BitGo;
using Xunit;

namespace Trakx.Data.Market.Tests.Unit.Sources.BitGo
{
    public class SupportedTokenProviderTests
    {
        [Fact]
        public void SupportedTokensBySymbol_should_read_from_csv()
        {
            var provider = new SupportedTokenProvider();
            provider.SupportedTokensBySymbol.Count.Should().Be(249);
        }
    }
}
