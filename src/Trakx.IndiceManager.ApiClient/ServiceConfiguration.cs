using System.Net.Http;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.IndiceManager.ApiClient
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddIndexManagerApiClient(
            this IServiceCollection serviceCollection, string baseUrl)
        {
            Guard.Against.NullOrEmpty(baseUrl, nameof(baseUrl));

            var clientHandler = new HttpClientHandler();

            //need to set the header extraction function of the Authorised client here, when ready.
            serviceCollection.AddSingleton<IIndiceCreationClient, IndiceCreationClient>(serviceProvider => new IndiceCreationClient(baseUrl, new HttpClient(clientHandler)));
            serviceCollection.AddSingleton<IIndiceSupplyClient, IndiceSupplyClient>(serviceProvider => new IndiceSupplyClient(baseUrl, new HttpClient(clientHandler)));
            serviceCollection.AddSingleton<IWrappingClient, WrappingClient>(serviceProvider => new WrappingClient(baseUrl, new HttpClient(clientHandler)));

            return serviceCollection;
        }
    }
}
