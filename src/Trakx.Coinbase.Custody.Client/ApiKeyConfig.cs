using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Trakx.Coinbase.Custody.Client.Interfaces;

namespace Trakx.Coinbase.Custody.Client
{
    public class ApiKeyConfig : IApiKeyConfig
    {
        private readonly string _apiKey;
        private readonly string _apiPassphrase;
        
        public ApiKeyConfig(string apiKey, string apiPassPhrase)
        {
            _apiKey = apiKey;
            _apiPassphrase = apiPassPhrase;
        }

        public void Configure(IFlurlClient client)
        {
            client.Configure(ApiKeyAuth);
        }

        private void ApiKeyAuth(ClientFlurlHttpSettings settings)
        {
            async Task SetHeaders(HttpCall http)
            {
                http.FlurlRequest
                    .WithHeader(HeaderNames.AccessKey, _apiKey)
                    .WithHeader(HeaderNames.AccessPassphrase, _apiPassphrase)
                    .WithHeader(HeaderNames.ContentType, "application/json");
            }

            settings.BeforeCallAsync = SetHeaders;
        }
    }
}
