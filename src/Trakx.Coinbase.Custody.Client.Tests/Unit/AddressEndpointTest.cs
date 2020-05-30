using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Coinbase.Custody.Client.Tests.Unit.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit
{
    public sealed class AddressEndpointTest : IDisposable
    {
        private readonly HttpTest _httpTest;
        private readonly string  _passPhrase = "evez6rv4156";
        private readonly string _apiKey = "atfratdrtygfyuhjiojko";
        private readonly IAddressEndpoint _addressEndpoint;

        public AddressEndpointTest()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCoinbaseLibrary(_apiKey, _passPhrase);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var apiConfig = serviceProvider.GetRequiredService<IApiKeyConfig>();
            var coinbaseClient=new CoinbaseClient(apiConfig);
            _addressEndpoint= new AddressEndpoint(coinbaseClient);
            _httpTest = new HttpTest();
        }

        public void Dispose()
        {
            _httpTest.Dispose();
        }

        [Fact]
        public async Task ListAddressesAsync_should_call_API_without_query_parameters()
        {
            await _addressEndpoint.ListAddressesAsync();
            _httpTest.ShouldHaveCorrectHeader(_apiKey, _passPhrase);
            _httpTest.ShouldHaveCalled("https://api.custody.coinbase.com/api/v1/addresses")
                .WithoutQueryParams("currency", "state");
        }
        
        [Fact]
        public async Task ListAddressesAsync_should_call_API_with_query_parameters()
        {
            await _addressEndpoint.ListAddressesAsync("btc", AddressState.Restored);
            _httpTest.ShouldHaveCorrectHeader(_apiKey, _passPhrase);
            _httpTest.ShouldHaveCalled("https://api.custody.coinbase.com/api/v1/addresses")
                .WithQueryParamValues(("btc", AddressState.Restored))
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task ListAddressesAsync_should_deserialize_response_to_model()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent<AddressResponse>();
            _httpTest.RespondWith(sampleResponse);

            var response = await _addressEndpoint.ListAddressesAsync("btc", AddressState.Restored);
            
            response.Data[0].Address.Should().Be("fake_btc_cold_address");
            response.Data[0].State.Should().Be(AddressState.Cold);
            response.Data[0].Balance.Should().Be(12);
            response.Data[0].BlockchainLink.Should().Be("https://live.blockcypher.com/btc/address/fake_btc_cold_address");
            response.Pagination.Before.Should().Be("fake_btc_cold_address");
            _httpTest.ShouldHaveCorrectHeader(_apiKey, _passPhrase);
        }

    }
}
