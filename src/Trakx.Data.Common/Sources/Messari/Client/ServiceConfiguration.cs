using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Common.Sources.Messari.Client
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddMessariClient(this IServiceCollection services)
        {
            services.AddSingleton<IMessariClient, MessariClient>();
            services.AddSingleton<RequestHelperFactory>();
            services.AddHttpClient<RequestHelper>("MessariRequestHelper");

            return services;
        }
    }
}
