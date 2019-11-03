using System;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Sources.Messari.Client
{
    public class MessariApiClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MessariApiClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MessariApiClient Create()
        {
            return _serviceProvider.GetRequiredService<MessariApiClient>();
        }
    }
}
