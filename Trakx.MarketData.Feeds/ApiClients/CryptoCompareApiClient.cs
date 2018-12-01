using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http.Extensions;

using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.ApiClients;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;
using Trakx.MarketData.Feeds.Models.CryptoCompare;

namespace Trakx.MarketData.Feeds.ApiClients
{
    public class CryptoCompareApiClient : ICryptoCompareApiClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public CryptoCompareApiClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <inheritdoc />
        public async Task<ICryptoCompareResponse> GetAllSupportedCoins()
        {
            using (var client = _clientFactory.CreateClient(ApiConstants.CryptoCompare.HttpClientName))
            {
                var queryParams = new Dictionary<string, string>()
                {
                    { ApiConstants.CryptoCompare.ApiKeyParam, ApiConstants.CryptoCompare.ApiKey }
                };

                var queryBuilder = new QueryBuilder(queryParams);
                queryBuilder.ToQueryString().ToUriComponent();
                var response = await client.GetAsync(ApiConstants.CryptoCompare.AllCoins + queryBuilder.ToQueryString().ToUriComponent());
                var contentAsString = await response.Content.ReadAsStringAsync();
                var deserialised = JsonConvert.DeserializeObject<CryptoCompareResponse>(contentAsString);
                return deserialised;
            }
        }
    }
}
