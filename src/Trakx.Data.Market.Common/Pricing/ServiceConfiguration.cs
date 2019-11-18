using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Market.Common.Indexes;

namespace Trakx.Data.Market.Common.Pricing
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddPricing(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IIndexDetailsProvider, IndexDetailsProvider>();
            serviceCollection.AddSingleton<NavCalculator>();
            return serviceCollection;
        }
    }
}
