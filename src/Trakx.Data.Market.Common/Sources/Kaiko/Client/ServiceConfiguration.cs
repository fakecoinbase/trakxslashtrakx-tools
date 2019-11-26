using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Sources.Kaiko.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddKaikoClient(this IServiceCollection services)
        {
            services.AddSingleton<IKaikoClient, KaikoClient>();
            services.AddSingleton<RequestHelperFactory>();
            services.AddHttpClient<RequestHelper>("KaikoRequestHelper", c =>
                c.DefaultRequestHeaders.Add(Constants.ApiHttpHeader, Constants.ApiKey));

            return services;
        }
    }
}
