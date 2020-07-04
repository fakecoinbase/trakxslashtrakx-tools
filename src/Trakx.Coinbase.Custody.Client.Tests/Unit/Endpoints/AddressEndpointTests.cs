using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Endpoints;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Coinbase.Custody.Client.Tests.Unit.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Endpoints
{
    public sealed class AddressEndpointTests : EndpointTests
    {
        private readonly AddressEndpoint _addressEndpoint;

        public AddressEndpointTests() : base("address")
        {
            _addressEndpoint = (AddressEndpoint)ServiceProvider.GetRequiredService<IAddressEndpoint>();
            SampleResponse = SampleResponseHelper.GetSampleResponseContent("AddressResponse").Result;
            HttpTest.RespondWith(SampleResponse);
        }

        [Fact]
        public async Task ListAddressesAsync_should_call_API_without_query_parameters()
        {
            await _addressEndpoint.ListAddressesAsync();
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithoutQueryParams("currency", "state", "before", "after")
                .WithQueryParams("limit");
        }

        [Fact]
        public async Task ListAddressesAsync_should_call_API_with_query_parameters()
        {
            await _addressEndpoint.ListAddressesAsync("btc", AddressState.Restored, new PaginationOptions(pageSize: 20, before: "xrp"));
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValues(("btc", AddressState.Restored, "xrp", 20))
                .WithQueryParams("currency", "state", "limit", "before")
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task ListAddressesAsync_should_deserialize_response_to_model()
        {
            var response = await _addressEndpoint.ListAddressesAsync("btc", AddressState.Restored);

            response.Data[0].Address.Should().Be("fake_btc_cold_address");
            response.Data[0].State.Should().Be(AddressState.Cold);
            response.Data[0].Balance.Should().Be(12);
            response.Data[0].CreatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 23, 494, TimeSpan.Zero));
            response.Data[0].UpdatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 23, 520, TimeSpan.Zero));
            response.Data[0].BlockchainLink.Should().Be("https://live.blockcypher.com/btc/address/fake_btc_cold_address");
            response.Pagination.Before.Should().Be("fake_btc_cold_address");
        }

        [Fact]
        public void ListAddressesAsync_should_throw_FlurlError_if_request_fail()
        {
            HttpTest.Dispose();
            HttpTest = new HttpTest();
            HttpTest.RespondWith(status: 404);
            Func<Task> request = async () => await _addressEndpoint.ListAddressesAsync();

            request.Should().ThrowExactly<FlurlHttpException>("Server respond with 404 status code.");
        }
    }
}
