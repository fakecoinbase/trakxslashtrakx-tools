using CryptoCompare;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Common.Sources.CryptoCompare
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCryptoCompareClient(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var apiKey = configuration.GetSection("ApiKeys").GetValue<string>("CryptoCompare");
                return new CryptoCompareClient(apiKey);
            });
            return services;
        }
    }
}
