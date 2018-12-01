using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http.Extensions;

using Newtonsoft.Json;

using Trakx.MarketData.Feeds.ApiClients;
using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;
using Trakx.MarketData.Feeds.Tests.Utils;

using Xunit;
using Xunit.Abstractions;

using Constants = Trakx.MarketData.Feeds.Common.ApiClients.ApiConstants.CryptoCompare;

namespace Trakx.MarketData.Feeds.Tests.ApiClients
{
    public class CryptoCompareApiClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private CryptoCompareApiClient _apiClient;
        private HttpClient _httpClient;

        public CryptoCompareApiClientTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "just a one off, but keep the code to do queries easily")]
        //[Fact]
        public async Task GetSampleApiResponse()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(Constants.SandboxEndpoint) };

            var queryParams = new Dictionary<string, string>()
              {
                  { Constants.ApiKeyParam, Constants.ApiKey }
              };

            var queryBuilder = new QueryBuilder(queryParams);
            queryBuilder.ToQueryString().ToUriComponent();
            var response = await _httpClient.GetAsync(Constants.AllCoins + queryBuilder.ToQueryString().ToUriComponent());
            var contentAsString = await response.Content.ReadAsStringAsync();
            var deserialised = JsonConvert.DeserializeObject<ICryptoCompareResponse>(contentAsString);
        }

        [Fact]
        public async Task ApiClient_Should_Deserialise_ICoinsAndMarketCapListing()
        {
            _apiClient = new CryptoCompareApiClient(HttpClientFactory.GetHttpClientFactory(
                "http://hello.com",
                () => {
                    var reponse = new HttpResponseMessage(HttpStatusCode.OK);
                    reponse.Content = new StreamContent(TestData.CryptoCompare.CoinListAsStream());
                    return reponse;
                }));

            var listing = await _apiClient.GetAllSupportedCoins();
            listing.Data.Count.Should().Be(3320);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
