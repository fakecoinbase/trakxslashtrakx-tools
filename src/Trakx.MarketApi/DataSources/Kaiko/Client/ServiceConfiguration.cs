using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Trakx.MarketApi.DataSources.Kaiko.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddKaikoClient(this IServiceCollection services)
        {
            services.AddSingleton<RequestHelper>();
            services.AddSingleton<KaikoApiClientFactory>();
            services.AddSingleton<JsonSerializer>();
            services.AddHttpClient<KaikoApiClient>(c =>
                c.DefaultRequestHeaders.Add(Constants.ApiHttpHeader, Constants.ApiKey));

            return services;
        }
    }
}
