using System.Net.Http;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.IndiceManager.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddApiClient(
            this IServiceCollection serviceCollection, string baseUrl)
        {
            Guard.Against.NullOrEmpty(baseUrl, nameof(baseUrl));

            var clientHandler = new HttpClientHandler{ServerCertificateCustomValidationCallback = (message, cert, chain,
                errors) => true}; //enable SSL certificate verification in developement

            serviceCollection.AddSingleton<IIndiceCreationClient, IndiceCreationClient>(serviceProvider => new IndiceCreationClient(baseUrl, new HttpClient(clientHandler)));
            serviceCollection.AddSingleton<IIndiceSupplyClient, IndiceSupplyClient>(serviceProvider => new IndiceSupplyClient(baseUrl, new HttpClient(clientHandler)));
            serviceCollection.AddSingleton<IWrappingClient, WrappingClient>(serviceProvider => new WrappingClient(baseUrl, new HttpClient(clientHandler)));
            
            return serviceCollection;
        }
    }
}
