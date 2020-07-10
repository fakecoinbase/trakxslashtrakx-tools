using System.Net.Http;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Exchange.ApiClient
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddTrakxExchangeApiClient(
            this IServiceCollection serviceCollection, string baseUrl)
        {
            Guard.Against.NullOrEmpty(baseUrl, nameof(baseUrl));

            var clientHandler = new HttpClientHandler();

            //need to set the header extraction function of the Authorised client here, when ready.
            serviceCollection.AddSingleton<IClient, Client>(serviceProvider => new Client(baseUrl, new HttpClient(clientHandler)));
            
            return serviceCollection;
        }
    }
}
