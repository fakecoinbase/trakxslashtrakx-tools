using Microsoft.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Sources.Messari.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddMessariClient(this IServiceCollection services)
        {
            services.AddSingleton<RequestHelper>();
            services.AddSingleton<MessariApiClientFactory>();
            services.AddHttpClient<MessariApiClient>();

            return services;
        }
    }
}
