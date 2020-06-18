using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Trakx.Coinbase.Custody.Client.Endpoints;
using Trakx.Coinbase.Custody.Client.Models;
using Trakx.Coinbase.Custody.Client.Tests.Unit.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Endpoints
{
    public class TransactionEndpointTests : EndpointTests
    {
        private readonly TransactionEndpoint _transactionEndpoint;

        public TransactionEndpointTests() : base("transactions")
        {
            _transactionEndpoint = new TransactionEndpoint(CoinbaseClient);
        }

        [Fact]
        public async Task ListTransactionsAsync_should_call_API_with_query_parameters()
        {
            await _transactionEndpoint.ListTransactionsAsync("eur", TransactionState.Gassing,limit:5);
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValues(new { currency = "eur", state = TransactionState.Gassing,limit=5 })
                .WithQueryParams("currency", "state","limit")
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task ListTransactionsAsync_should_call_API_without_query_parameters()
        {
            await _transactionEndpoint.ListTransactionsAsync();
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithoutQueryParams();
        }

        [Fact]
        public async Task ListTransactionsAsync_should_deserialize_response_to_model()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent("PagedResponseTransaction");
            HttpTest.RespondWith(sampleResponse);

            var response = await _transactionEndpoint.ListTransactionsAsync();

            response.Pagination.After.Should().Be("820d3373-a050-435e-85d3-72f3f9a15f03");
            response.Data[0].Id.Should().Be("08396aef-3126-4ed4-b109-46b82ff1857b");
            response.Data[0].State.Should().Be("done");
            response.Data[0].CreatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 29, 787, TimeSpan.Zero));
            response.Data[0].UpdatedAt.Should().Be(new DateTimeOffset(2020, 7, 2, 18, 15, 29, 787, TimeSpan.Zero));
            response.Data[0].Amount.Should().Be(-400);
            response.Data[0].Hashes[0].Should().Be("hash1");
            response.Data[0].WalletId.Should().Be("6b4be175-793f-4d99-b922-b90e8b6677be");
        }

        [Fact]
        public void GetTransactionAsync_should_return_error_if_parameter_null_or_empty()
        {
            Func<Task> nullAction = async () => await _transactionEndpoint.GetTransactionAsync(null);
            nullAction.Should().ThrowExactly<ArgumentNullException>();

            Func<Task> emptyAction = async () => await _transactionEndpoint.GetTransactionAsync("");
            emptyAction.Should().ThrowExactly<ArgumentException>();

            HttpTest.ShouldNotHaveMadeACall();
        }

        [Fact]
        public async Task GetTransactionAsync_should_call_API_with_correct_path()
        {
            await _transactionEndpoint.GetTransactionAsync("2");
            HttpTest.ShouldHaveCalled(Url.Combine(EndpointUrl, "2"))
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task GetTransactionAsync_should_deserialize_response_to_model()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent("Transaction");
            HttpTest.RespondWith(sampleResponse);

            var response = await _transactionEndpoint.GetTransactionAsync("2");

            response.Id.Should().Be("08396aef-3126-4ed4-b109-46b82ff1857b");
            response.State.Should().Be("done");
            response.CreatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 29, 787, TimeSpan.Zero));
            response.UpdatedAt.Should().Be(new DateTimeOffset(1998, 3, 1, 18, 15, 29, 787, TimeSpan.Zero));
            response.Amount.Should().Be(-400);
            response.Hashes.Should().BeEmpty();
            response.WalletId.Should().Be("6b4be175-793f-4d99-b922-b90e8b6677be");
        }

    }
}
