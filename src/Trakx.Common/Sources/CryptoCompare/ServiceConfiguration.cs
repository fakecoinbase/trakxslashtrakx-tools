using System;
using CryptoCompare;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Interfaces;

namespace Trakx.Common.Sources.CryptoCompare
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCryptoCompareClient(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var apiKey = GetApiKeyFromConfiguration(provider);
                return new CryptoCompareClient(apiKey);
            });

            services.AddTransient<IClientWebsocket, WrappedClientWebsocket>();
            services.AddTransient<IWebSocketStreamer, WebSocketStreamer>();
            services.AddSingleton<IApiDetailsProvider, ApiDetailsProvider>();
            services.AddSingleton<WebSocketClient>();
            return services;
        }

        private static string GetApiKeyFromConfiguration(IServiceProvider provider)
        {
            var configuration = provider.GetService<IConfiguration>();
            var apiKey = configuration.GetSection("ApiKeys").GetValue<string>("CryptoCompare");
            return apiKey;
        }
    }
}
