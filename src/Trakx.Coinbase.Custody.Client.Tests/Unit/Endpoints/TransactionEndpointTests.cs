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
    public class TransactionEndpointTests : EndpointTests
    {
        private readonly TransactionEndpoint _transactionEndpoint;

        public TransactionEndpointTests() : base("transactions")
        {
            _transactionEndpoint = (TransactionEndpoint)ServiceProvider.GetRequiredService<ITransactionEndpoint>();
            SampleResponse = SampleResponseHelper.GetSampleResponseContent("PagedResponseTransaction").GetAwaiter().GetResult();
        }

        [Fact]
        public async Task ListTransactionsAsync_should_call_API_with_query_parameters()
        {
            HttpTest.RespondWith(SampleResponse);
            await _transactionEndpoint.ListTransactionsAsync("eur", TransactionState.gassing, paginationOptions: new PaginationOptions(pageSize: 5));
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValues(new { currency = "eur", state = TransactionState.gassing, limit = 5 })
                .WithQueryParams("currency", "state", "limit")
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async Task ListTransactionsAsync_should_call_API_without_query_parameters()
        {
            HttpTest.RespondWith(SampleResponse);
            await _transactionEndpoint.ListTransactionsAsync();
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValue("limit", 25);
        }

        [Fact]
        public async Task ListTransactionsAsync_should_deserialize_response_to_model()
        {
            HttpTest.RespondWith(SampleResponse);
            var response = await _transactionEndpoint.ListTransactionsAsync(paginationOptions: new PaginationOptions(pageSize: 20), currency: "btc");

            response.Pagination.After.Should().Be("820d3373-a050-435e-85d3-72f3f9a15f03");
            VerifyTransaction(response.Data[0]);
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
            SampleResponse =SampleResponseHelper.GetSampleResponseContent("Transaction").Result;
            HttpTest.RespondWith(SampleResponse);
            var response = await _transactionEndpoint.GetTransactionAsync("08396aef-3126-4ed4-b109-46b82ff1857b");

            VerifyTransaction(response);
        }

        private static void VerifyTransaction(CoinbaseTransaction response)
        {
            response.Id.Should().Be("8937cba9-2e49-43cb-aaa1-3b06e0d74d52");
            response.State.Should().Be(TransactionState.done);
            response.Type.Should().Be(TransactionType.withdrawal);
            response.CreatedAt.Should().Be(new DateTimeOffset(2019, 8, 19, 18, 15, 29, 787, TimeSpan.Zero));
            response.UpdatedAt.Should().Be(new DateTimeOffset(1998, 3, 1, 18, 15, 29, 751, TimeSpan.Zero));
            response.UnscaledAmount.Should().Be(100);
            response.Hashes.Should().Contain("ccf2459f5fce52834c5bb5bbcfa93ff3");
            response.WalletId.Should().Be("5d57dd0b-50c0-4c06-bfbd-b31f58a25a8a");
            response.Amount.Should().Be(0.000001m);
            response.Fee.Should().Be(10.30665m);
            response.Source.Should().BeNullOrEmpty();
        }
    }
}
