using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Sources.Kaiko.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddKaikoClient(this IServiceCollection services)
        {
            services.AddSingleton<IRequestHelper, RequestHelper>();
            services.AddSingleton<KaikoApiClientFactory>();
            services.AddHttpClient<KaikoApiClient>(c =>
                c.DefaultRequestHeaders.Add(Constants.ApiHttpHeader, Constants.ApiKey));

            return services;
        }
    }
}
