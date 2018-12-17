using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Trakx.MarketData.Feeds.ApiClients;

using Xunit;
using Xunit.Abstractions;

using Constants = Trakx.MarketData.Feeds.Common.ApiClients.ApiConstants.CoinMarketCap;

namespace Trakx.MarketData.Feeds.Tests.ApiClients
{
    public class CoinMarketCapApiClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private CoinMarketCapApiClient _apiClient;
        private HttpClient _httpClient;

        public CoinMarketCapApiClientTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "just a one off, but keep the code to do queries easily")]
        public async Task GetSampleApiResponse()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(Constants.SandboxEndpoint) };
            _httpClient.DefaultRequestHeaders.Add(Constants.ApiKeyHeader, Constants.ApiKeySandbox);

            var response = await _httpClient.GetAsync(Constants.MarketCapLatest);
            _output.WriteLine(response.ToString());
            var contentAsString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentAsString);
        }

        [Fact]
        public async Task ApiClient_Should_Deserialise_ICoinsAndMarketCapListing()
        {
            _apiClient = new CoinMarketCapApiClient(Utils.HttpClientFactory.GetHttpClientFactory(
                "http://hello.com",
                () => {
                    var reponse = new HttpResponseMessage(HttpStatusCode.OK);
                    reponse.Content = new StreamContent(TestData.CoinMarketCap.CoinListAsStream());
                    return reponse;
                }));

            var listing = await _apiClient.GetCoinsAndMarketCapListings();
            listing.CoinsAndMarketCaps.Length.Should().Be(100);
            listing.Status.Elapsed.Should().Be(12);
            listing.CoinsAndMarketCaps.First().Name.Should().Be("Bitcoin");
            listing.CoinsAndMarketCaps.First().Quote.Should().ContainKey("USD");
            listing.CoinsAndMarketCaps.First().Quote["USD"].MarketCap.Should().Be(111774707273.6615m);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
