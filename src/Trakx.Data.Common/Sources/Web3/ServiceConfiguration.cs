using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trakx.Data.Common.Sources.Messari.Client;
using Trakx.Data.Common.Sources.Web3.Client;

namespace Trakx.Data.Common.Sources.Web3
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
        public static IServiceCollection AddWeb3Client(this IServiceCollection services,
            string infuraApiKey = null)
        {
            services.AddSingleton<IWeb3Client, Web3Client>(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var logger = serviceProvider.GetService<ILogger<Web3Client>>();
                var apiKey = configuration?["INFURA_API_KEY"] ?? infuraApiKey;
                return new Web3Client(apiKey, logger);
            });

            return services;
        }
    }
}
