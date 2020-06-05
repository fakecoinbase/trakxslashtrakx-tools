using System;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Coinbase.Custody.Client.Tests.Unit.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit
{
    public class CoinbaseClientTests : IDisposable
    {
        private readonly ICoinbaseClient _client;
        private readonly HttpTest _httpTest;
        public CoinbaseClientTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCoinbaseLibrary("apiKey", "fhzycdushbvfc");

            _httpTest=new HttpTest();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _client = serviceProvider.GetRequiredService<ICoinbaseClient>();
        }

        public void Dispose()
        {
            _httpTest.Dispose();
        }

        [Fact]
        public void  ListAddressesAsync_should_throw_FlurlError_is_request_fail()
        {
            _httpTest.RespondWith(status: 404);
            Func<Task> request = async () => await _client.ListAddressesAsync();

            request.Should().ThrowExactly<FlurlHttpException>("Server respond with 404 status code.");
        }

        [Fact]
        public async Task ListAddressesAsync_should_return_List_of_addresses()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent("AddressResponse");
            _httpTest.RespondWith(sampleResponse);

            var result = await _client.ListAddressesAsync();
            result.Should().BeOfType<PagedResponse<AddressResponse>>();
        }
    }
}
