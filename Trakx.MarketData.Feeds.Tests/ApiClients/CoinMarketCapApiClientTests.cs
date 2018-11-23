using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Trakx.MarketData.Feeds.Common.ApiClients;

using Xunit;
using Xunit.Abstractions;

using Constants = Trakx.MarketData.Feeds.Common.ApiClients.ApiConstants.CoinMarketCap;

namespace Trakx.MarketData.Feeds.Tests.ApiClients
{
    public class CoinMarketCapApiClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private HttpClient _apiClient;

        public CoinMarketCapApiClientTests(ITestOutputHelper output)
        {
            _output = output;
            _apiClient = new HttpClient { BaseAddress = new Uri(Constants.SandboxEndpoint) };
            _apiClient.DefaultRequestHeaders.Add(Constants.ApiKeyHeader, Constants.ApiKeySandbox);
        }

        [Fact(Skip = "just a one off, but keep the code to do queries easily")]
        public async Task GetSampleApiResponse()
        {
            var response = await _apiClient.GetAsync(Constants.LatestMarketCap);
            _output.WriteLine(response.ToString());
            var contentAsString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(contentAsString);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _apiClient?.Dispose();
        }
    }
}
