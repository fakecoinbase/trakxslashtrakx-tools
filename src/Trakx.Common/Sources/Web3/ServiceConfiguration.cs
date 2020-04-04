using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Sources.Web3.Client;

namespace Trakx.Common.Sources.Web3
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddWeb3Client(this IServiceCollection services)
        {
            services.AddSingleton<IWeb3Client, Web3Client>();
            return services;
        }
    }
}
