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
    public class CurrencyEndpointTest : EndpointTests
    {
        private readonly CurrencyEndpoint _currencyEndpoint;


        public CurrencyEndpointTest() : base("currencies")
        {
            _currencyEndpoint = (CurrencyEndpoint)ServiceProvider.GetRequiredService<ICurrencyEndpoint>();
            SampleResponse = SampleResponseHelper.GetSampleResponseContent("PagedResponseCurrency").Result;
            HttpTest.RespondWith(SampleResponse);
        }


        [Fact]
        public async Task ListCurrenciesAsync_should_deserialize_response_to_model()
        {
            var response = await _currencyEndpoint.ListCurrenciesAsync();

            response.Data[0].Symbol.Should().Be("btc");
            response.Data[1].Symbol.Should().Be("bch");
            response.Data[0].Name.Should().Be("Bitcoin");
            response.Data[1].Name.Should().Be("Bitcoin Cash");
            response.Data[0].Decimals.Should().Be(8);
            response.Data[1].Decimals.Should().Be(12);

            response.Pagination.Before.Should().Be("btc");
        }

        [Fact]
        public async Task ListCurrenciesAsync_should_call_API_without_query_parameters()
        {
            await _currencyEndpoint.ListCurrenciesAsync();
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithoutQueryParams("before", "after")
                //.WithQueryParamValue("limit", 25)
                ;
        }

        [Fact]
        public async Task ListCurrenciesAsync_should_call_API_with_query_parameters()
        {
            await _currencyEndpoint.ListCurrenciesAsync(new PaginationOptions(pageSize: 20, before: "btc"));
            HttpTest.ShouldHaveCalled(EndpointUrl)
                .WithQueryParamValues(("btc", 20))
                .WithQueryParams("limit", "before")
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public void GetCurrencyAsync_should_return_error_if_parameter_null_or_empty()
        {
            Func<Task> nullAction = async () => await _currencyEndpoint.GetCurrencyAsync(null);
            nullAction.Should().ThrowExactly<ArgumentNullException>();

            Func<Task> emptyAction = async () => await _currencyEndpoint.GetCurrencyAsync("");
            emptyAction.Should().ThrowExactly<ArgumentException>();

            HttpTest.ShouldNotHaveMadeACall();
        }

        [Fact]
        public async Task GetCurrencyAsync_should_call_API_with_correct_path()
        {
            await _currencyEndpoint.GetCurrencyAsync("btc");
            HttpTest.ShouldHaveCalled(Url.Combine(EndpointUrl, "btc"))
                .WithVerb(HttpMethod.Get);
        }
    }
}
