using FluentAssertions;
using Nethereum.Util;
using Xunit;

namespace Trakx.Tests.Unit.Common.Extensions
{
    public class IsValidEthereumAddressTests
    {
        [Fact]
        public void MyTestedMethod_Should_Be_Producing_This_Result_When_Some_Conditions_Are_Met()
        {
            "0x43cE8afa6985C86485640c7FEC81bc8FDd66E95f".IsValidEthereumAddressHexFormat().Should().BeTrue();
            "0x43cE8afa6985C86485640c7FEC81bc8FDd66E9f".IsValidEthereumAddressHexFormat().Should().BeFalse();
        }
    }
}