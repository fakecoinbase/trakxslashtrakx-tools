using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Coinbase.Custody.Client.Endpoints;
using Trakx.Coinbase.Custody.Client.Tests.Unit.Models;
using Xunit;

namespace Trakx.Coinbase.Custody.Client.Tests.Unit.Endpoints
{
    public class CurrencyEndpointTest :EndpointTests
    {
        private readonly CurrencyEndpoint _currencyEndpoint;

        public CurrencyEndpointTest() : base("currencies")
        {
            _currencyEndpoint = new CurrencyEndpoint(CoinbaseClient);
        }


        [Fact]
        public async Task ListCurrenciesAsync_should_deserialize_response_to_model()
        {
            var sampleResponse = await SampleResponseHelper.GetSampleResponseContent("PagedResponseCurrency");
            HttpTest.RespondWith(sampleResponse);

            var response = await _currencyEndpoint.ListCurrenciesAsync();

            response.Data[0].Symbol.Should().Be("btc");
            response.Data[1].Symbol.Should().Be("bch");
            response.Data[0].Name.Should().Be("Bitcoin");
            response.Data[1].Name.Should().Be("Bitcoin Cash");
            response.Data[0].Decimals.Should().Be(8);
            response.Data[1].Decimals.Should().Be(12);

            response.Pagination.Before.Should().Be("btc");
        }
    }
}
