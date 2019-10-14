using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Trakx.MarketApi.DataSources.Kaiko.AggregatedPrice;

namespace Trakx.MarketApi.DataSources.Kaiko.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddKaikoClient(this IServiceCollection services)
        {
            services.AddSingleton<AggregatedPriceRequest>();
            services.AddSingleton<KaikoApiClientFactory>();
            services.AddSingleton<JsonSerializer>();
            services.AddHttpClient<KaikoApiClient>(c =>
                c.DefaultRequestHeaders.Add(Constants.ApiHttpHeader, Constants.ApiKey));

            return services;
        }
    }
}
