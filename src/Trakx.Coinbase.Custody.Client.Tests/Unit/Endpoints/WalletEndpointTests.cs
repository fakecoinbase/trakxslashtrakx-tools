using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Trakx.Coinbase.Custody.Client.Endpoints;
using Trakx.Coinbase.Custody.Client.Tests.Unit.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Endpoints
{
    public class WalletEndpointTests : EndpointTests
    {
        private readonly WalletEndpoint _walletEndpoint;

        public WalletEndpointTests() : base("wallets")
        {
            _walletEndpoint = new WalletEndpoint(CoinbaseClient);
        }

        [Fact]
        public async Task ListWalletsAsync_should_call_API_with_query_parameters()
        {
            await _walletEndpoint.ListWalletsAsync("eur");
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValue("currency", "eur")
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task ListWalletsAsync_should_call_API_without_query_parameters()
        {
            await _walletEndpoint.ListWalletsAsync();
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithoutQueryParams();
        }

        [Fact]
        public async Task ListWalletsAsync_should_deserialize_response_to_model()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent("PagedResponseWallet");
            HttpTest.RespondWith(sampleResponse);

            var response = await _walletEndpoint.ListWalletsAsync();

            response.Pagination.After.Should().Be("97881a38-fba1-4563-992c-130a306c5a14");
            response.Data[0].Id.Should().Be("575f6845-9508-4cd7-a794-cb9ab7faf441");
            response.Data[0].Name.Should().Be("test");
            response.Data[0].CreatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 34, 803, TimeSpan.Zero));
            response.Data[0].UpdatedAt.Should().Be(new DateTimeOffset(1998, 8, 10, 18, 15, 34, 803, TimeSpan.Zero));
            response.Data[0].Balance.Should().Be("1.26");
            response.Data[0].WithdrawableBalance.Should().Be("2.235");
            response.Data[0].ColdAddress.Should().Be("fake_btc_cold_address");
            response.Data[0].Currency.Should().Be("btc");
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
            await _walletEndpoint.GetWalletAsync("2");
            HttpTest.ShouldHaveCalled(Url.Combine(EndpointUrl, "2"))
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task GetWalletAsync_should_deserialize_response_to_model()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent("Wallet");
            HttpTest.RespondWith(sampleResponse);

            var response = await _walletEndpoint.GetWalletAsync("5");

            response.Id.Should().Be("575f6845-9508-4cd7-a794-cb9ab7faf441");
            response.Name.Should().Be("test");
            response.CreatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 34, 803, TimeSpan.Zero));
            response.UpdatedAt.Should().Be(new DateTimeOffset(2000, 1, 1, 18, 15, 34, 803, TimeSpan.Zero));
            response.Balance.Should().Be("2.23");
            response.WithdrawableBalance.Should().Be("1.25");
            response.ColdAddress.Should().Be("fake_btc_cold_address");
        }
    }
}
