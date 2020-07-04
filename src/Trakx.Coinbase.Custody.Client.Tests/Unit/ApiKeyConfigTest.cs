using System;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Flurl.Http.Testing;


namespace Trakx.Coinbase.Custody.Client.Tests.Unit
{
    public class ApiKeyConfigTest
    {
        private readonly HttpTest _httpTest;

        public ApiKeyConfigTest()
        {
            _httpTest = new HttpTest();
        }
        [Fact]
        public void ApiKeyConfig_should_return_error_if_apiKey_passphrase_are_empty()
        {

            var serviceCollection = new ServiceCollection();
            Action act = () => serviceCollection.AddCoinbaseLibrary("", "");
            act.Should().ThrowExactly<ArgumentException>();

        }

        [Fact]
        public void ApiKeyConfig_should_return_error_if_apiKey_passphrase_are_null()
        {
            var serviceCollection = new ServiceCollection();
            Action act = () => serviceCollection.AddCoinbaseLibrary(null, null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}