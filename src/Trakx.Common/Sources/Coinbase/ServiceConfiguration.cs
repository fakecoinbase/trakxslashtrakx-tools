﻿using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Common.Sources.Coinbase
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddCoinbaseClient(this IServiceCollection services)
        {
            services.AddSingleton<ICoinbaseClient, CoinbaseClient>();

            return services;
        }
    }
}