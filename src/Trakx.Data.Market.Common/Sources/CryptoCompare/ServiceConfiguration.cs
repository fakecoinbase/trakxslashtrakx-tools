using CryptoCompare;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Sources.CryptoCompare
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCryptoCompareClient(this IServiceCollection services)
        {
            services.AddSingleton(_ => new CryptoCompareClient(Constants.ApiKey));
            return services;
        }
    }
}
