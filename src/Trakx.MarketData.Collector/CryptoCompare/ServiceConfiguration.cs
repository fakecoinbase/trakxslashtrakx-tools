using System;
using CryptoCompare;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Interfaces;

namespace Trakx.MarketData.Collector.CryptoCompare
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCryptoCompareClient(this IServiceCollection services)
        {
            services.AddSingleton<IApiDetailsProvider, ApiDetailsProvider>();
            services.AddSingleton<ICryptoCompareClient, CryptoCompareClient>(provider =>
            {
                var apiKey = provider.GetService<IApiDetailsProvider>().ApiKey;
                return new CryptoCompareClient(apiKey);
            });

            services.AddTransient<IClientWebsocket, WrappedClientWebsocket>();
            services.AddTransient<IWebSocketStreamer, WebSocketStreamer>();
            services.AddSingleton<ICryptoCompareWebSocketClient, CryptoCompareWebSocketClient>();
            services.AddSingleton<WrappedClientWebsocket>();
            
            return services;
        }
    }
}
