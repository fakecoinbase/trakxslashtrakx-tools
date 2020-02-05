using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Common.Sources.CoinGecko
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCoinGeckoClient(this IServiceCollection services)
        {
            services.AddSingleton<ClientFactory>();
            services.AddSingleton<ICoinGeckoClient, CoinGeckoClient>();
            services.AddHttpClient<global::CoinGecko.Clients.CoinsClient>("CoinsClient");
            services.AddHttpClient<global::CoinGecko.Clients.SimpleClient>("SimpleClient");
            return services;
        }
    }
}