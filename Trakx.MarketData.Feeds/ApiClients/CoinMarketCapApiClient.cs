using System.Net.Http;
using System.Threading.Tasks;

using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;
using Trakx.MarketData.Feeds.Models.CoinMarketCap;

namespace Trakx.MarketData.Feeds.ApiClients
{
    public class CoinMarketCapApiClient : ICoinMarketCapApiClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public CoinMarketCapApiClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<ICoinsAndMarketCapListing> GetCoinsAndMarketCapListings()
        {
            using (var client = _clientFactory.CreateClient(ApiConstants.CoinMarketCap.HttpClientName))
            {
                var response = await client.GetAsync(ApiConstants.CoinMarketCap.LatestMarketCap);
                var contentAsString = await response.Content.ReadAsStringAsync();
                var deserialised = CoinsAndMarketCapListing.FromJson(contentAsString);
                return deserialised;
            }
        }
    }
}
