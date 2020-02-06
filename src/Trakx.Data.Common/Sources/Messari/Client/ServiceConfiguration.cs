using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Common.Sources.Messari.Client
{
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Add dependencies required by the Messari client to the service collection.
        /// </summary>
        /// <param name="services">The service collection to which we add the registrations</param>
        /// <param name="apiKey">This parameter is only meant to be used when testing, for production and
        /// normal instanciation, the api key should come from environment variables.</param>
        /// <returns>The service collection, enriched with new dependencies allowing to resolve IMessariClient
        /// in the rest of the program.</returns>
        public static IServiceCollection AddMessariClient(this IServiceCollection services,
            string apiKey = null)
        {
            services.AddSingleton<IMessariClient, MessariClient>();
            services.AddSingleton<RequestHelperFactory>();
            services.AddHttpClient<RequestHelper>("MessariRequestHelper", (serviceProvider, client) =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var messariApiKey = configuration?["MESSARI_API_KEY"] ?? apiKey;
                client.DefaultRequestHeaders.Add("x-messari-api-key", messariApiKey);
            });

            return services;
        }
    }
}
