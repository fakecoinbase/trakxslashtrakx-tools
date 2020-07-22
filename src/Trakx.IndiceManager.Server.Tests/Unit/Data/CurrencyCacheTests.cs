using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Trakx.IndiceManager.Server.Tests.Unit.Data
{
    public class CurrencyCacheTests
    {
        [Fact]
        public void GetCurrency_should_not_call_currency_endpoint_when_currency_was_cached()
        {
            
        }

        [Fact]
        public void GetCurrency_should_call_currency_endpoint_when_currency_was_not_cached()
        {

        }

        [Fact]
        public void GetCurrency_should_cache_currency_when_currency_was_not_cached()
        {

        }
    }
}