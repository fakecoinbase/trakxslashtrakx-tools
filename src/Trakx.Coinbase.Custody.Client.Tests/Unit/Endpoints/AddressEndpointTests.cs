using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Coinbase.Custody.Client.Endpoints;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Coinbase.Custody.Client.Tests.Unit.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Endpoints
{
    public sealed class AddressEndpointTests : EndpointTests
    {
        private readonly AddressEndpoint _addressEndpoint;

        public AddressEndpointTests() : base("addresses")
        {
            _addressEndpoint = new AddressEndpoint(CoinbaseClient);
        }

        [Fact]
        public async Task ListAddressesAsync_should_call_API_without_query_parameters()
        {
            await _addressEndpoint.ListAddressesAsync();
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithoutQueryParams("currency", "state","limit","before","after");
        }

        [Fact]
        public async Task ListAddressesAsync_should_call_API_with_query_parameters()
        {
            await _addressEndpoint.ListAddressesAsync("btc", AddressState.Restored,"xrp", limit: 20);
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValues(("btc", AddressState.Restored,"xrp",20))
                .WithQueryParams("currency", "state","limit","before")
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task ListAddressesAsync_should_deserialize_response_to_model()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent("AddressResponse");
            HttpTest.RespondWith(sampleResponse);

            var response = await _addressEndpoint.ListAddressesAsync("btc", AddressState.Restored);

            response.Data[0].Address.Should().Be("fake_btc_cold_address");
            response.Data[0].State.Should().Be(AddressState.Cold);
            response.Data[0].Balance.Should().Be(12);
            response.Data[0].CreatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 23, 494, TimeSpan.Zero));
            response.Data[0].UpdatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 23, 520, TimeSpan.Zero));
            response.Data[0].BlockchainLink.Should().Be("https://live.blockcypher.com/btc/address/fake_btc_cold_address");
            response.Pagination.Before.Should().Be("fake_btc_cold_address");
        }


    }
}
