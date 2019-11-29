using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Market.Common.Indexes;

namespace Trakx.Data.Market.Common.Pricing
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddPricing(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IIndexDetailsProvider, IndexDetailsProvider>();
            serviceCollection.AddSingleton<INavCalculator, NavCalculator>();
            serviceCollection.AddSingleton<INavUpdater, NavUpdater>();
            return serviceCollection;
        }
    }
}
