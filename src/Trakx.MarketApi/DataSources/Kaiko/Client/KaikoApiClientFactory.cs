using System;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.MarketApi.DataSources.Kaiko.Client
{
    public class KaikoApiClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public KaikoApiClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public KaikoApiClient Create()
        {
            return _serviceProvider.GetRequiredService<KaikoApiClient>();
        }
    }
}
