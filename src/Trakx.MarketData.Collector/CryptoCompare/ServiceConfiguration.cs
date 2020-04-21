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
            services.AddSingleton<ICryptoCompareClient, CryptoCompareClient>(provider =>
            {
                var apiKey = Environment.GetEnvironmentVariable("CRYPTOCOMPARE_API_KEY");
                return new CryptoCompareClient(apiKey);
            });

            services.AddTransient<IClientWebsocket, WrappedClientWebsocket>();
            services.AddTransient<IWebSocketStreamer, WebSocketStreamer>();
            services.AddSingleton<IApiDetailsProvider, ApiDetailsProvider>();
            services.AddSingleton<ICryptoCompareWebSocketClient, CryptoCompareWebSocketClient>();
            services.AddSingleton<WrappedClientWebsocket>();
            
            return services;
        }
    }
}
