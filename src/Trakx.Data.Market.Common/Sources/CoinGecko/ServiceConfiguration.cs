using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Sources.CoinGecko
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCoinGeckoClient(this IServiceCollection services)
        {
            services.AddSingleton<ICoinGeckoClient, CoinGeckoClient>();
            services.AddHttpClient<ICoinGeckoClient>("CoinGeckoClients");

            return services;
        }
    }
}