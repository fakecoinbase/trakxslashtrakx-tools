using CryptoCompare;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Sources.CryptoCompare
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCryptoCompareClient(this IServiceCollection services)
        {
            services.AddSingleton(_ => 
                new CryptoCompareClient("5f95e17ff4599da5bc6f4b309c2e0b27d3a73ddfaba843a63be66be7ebc3e79e"));

            return services;
        }
    }
}
