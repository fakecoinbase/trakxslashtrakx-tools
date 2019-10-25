using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
namespace Trakx.MarketApi.DataSources.Messari.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddMessariClient(this IServiceCollection services)
        {
            services.AddSingleton<RequestHelper>();
            services.AddSingleton<MessariApiClientFactory>();
            services.AddSingleton<JsonSerializer>();
            services.AddHttpClient<MessariApiClient>();

            return services;
        }
    }
}
