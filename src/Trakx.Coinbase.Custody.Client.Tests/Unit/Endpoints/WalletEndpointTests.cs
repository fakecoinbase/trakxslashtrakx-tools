using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Coinbase.Custody.Client.Endpoints;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Coinbase.Custody.Client.Tests.Unit.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Endpoints
{
    public class WalletEndpointTests : EndpointTests
    {
        private readonly WalletEndpoint _walletEndpoint;

        public WalletEndpointTests() : base("wallets")
        {
            _walletEndpoint = (WalletEndpoint)ServiceProvider.GetRequiredService<IWalletEndpoint>();
            SampleResponse = SampleResponseHelper.GetSampleResponseContent("PagedResponseWallet").Result;
        }

        [Fact]
        public async Task ListWalletsAsync_should_call_API_with_query_parameters()
        {
            HttpTest.RespondWith(SampleResponse);
            await _walletEndpoint.ListWalletsAsync("eur", new PaginationOptions(pageSize: 50, before: "wallet1"));
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValues(new { currency = "eur", before = "wallet1", limit = 50 })
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task ListWalletsAsync_should_call_API_without_query_parameters()
        {
            HttpTest.RespondWith(SampleResponse);
            await _walletEndpoint.ListWalletsAsync();
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValue("limit", 25);
        }

        [Fact]
        public async Task ListWalletsAsync_should_deserialize_response_to_model()
        {
            HttpTest.RespondWith(SampleResponse);
            var response = await _walletEndpoint.ListWalletsAsync(paginationOptions: new PaginationOptions(pageSize: 2));

            response.Pagination.After.Should().Be("d3c50a5d-6abc-47dd-8300-36c4ed837504");
            response.Pagination.Before.Should().Be("32b34b7f-1555-4701-b017-e4c93ff9eaf7");

            PagedResponseWalletTest.CheckFirstWalletFromResponse(response.Data[0]);
            
        }

        [Fact]
        public void GetWalletAsync_should_return_error_if_parameter_null_or_empty()
        {
            Func<Task> nullAction = async () => await _walletEndpoint.GetWalletAsync(null);
            nullAction.Should().ThrowExactly<ArgumentNullException>();

            Func<Task> emptyAction = async () => await _walletEndpoint.GetWalletAsync("");
            emptyAction.Should().ThrowExactly<ArgumentException>();

            HttpTest.ShouldNotHaveMadeACall();
        }

        [Fact]
        public async Task GetWalletAsync_should_call_API_with_correct_path()
        {
            HttpTest.RespondWith(SampleResponse);
            await _walletEndpoint.GetWalletAsync("2");
            HttpTest.ShouldHaveCalled(Url.Combine(EndpointUrl, "2"))
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task GetWalletAsync_should_deserialize_response_to_model()
        {
            SampleResponse = SampleResponseHelper.GetSampleResponseContent("Wallet").Result;
            HttpTest.RespondWith(SampleResponse);
            var response = await _walletEndpoint.GetWalletAsync("5");

            PagedResponseWalletTest.CheckFirstWalletFromResponse(response);
        }
    }
}
