using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using FluentAssertions;
using Flurl.Http;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Flurl.Http.Testing;
using NSubstitute.ExceptionExtensions;
using Trakx.Coinbase.Custody.Client.Interfaces;


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
            act.Should().ThrowExactly<System.ArgumentException>();

        }

        [Fact]
        public void ApiKeyConfig_should_return_error_if_apiKey_passphrase_are_null()
        {
            var serviceCollection = new ServiceCollection();
            Action act = () => serviceCollection.AddCoinbaseLibrary(null, null);
            act.Should().ThrowExactly<System.ArgumentNullException>();
        }

        [Fact]
        public async Task ApiKeyConfig_should_pass_ApiKey_and_Passphrase_to_CoinbaseClient()
        {
            var passPhrase = "evez6rv4156";
            var apiKey = "atfratdrtygfyuhjiojko";

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCoinbaseLibrary(apiKey, passPhrase);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var apiConfig = serviceProvider.GetRequiredService<IApiKeyConfig>();

            var client = new CoinbaseClient(apiConfig);

            await client.ApiUrl
                .WithClient(client)
                .GetJsonAsync();

            _httpTest.ShouldHaveCalled("https://api.custody.coinbase.com/api/v1/")
                .WithContentType("application/json")
                .WithHeader(HeaderNames.AccessKey, apiKey)
                .WithHeader(HeaderNames.AccessPassphrase, passPhrase);
        }
    }
}