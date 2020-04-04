using System;
using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Common.Sources.Messari.Client
{
    public class RequestHelperFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RequestHelperFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public RequestHelper Create()
        {
            return _serviceProvider.GetRequiredService<RequestHelper>();
        }
    }
}
