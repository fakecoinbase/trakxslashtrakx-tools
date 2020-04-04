using System;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Common.Sources.CoinGecko
{
    public class ClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public global::CoinGecko.Interfaces.ICoinsClient CreateCoinsClient()
        {
            return _serviceProvider.GetRequiredService<global::CoinGecko.Clients.CoinsClient>();
        }

        public global::CoinGecko.Interfaces.ISimpleClient CreateSimpleClient()
        {
            return _serviceProvider.GetRequiredService<global::CoinGecko.Clients.SimpleClient>();
        }
    }
}
