using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Common.Pricing
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddPricing(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IIndexDefinitionProvider, IndexDefinitionProvider>();
            serviceCollection.AddSingleton<INavCalculator, NavCalculator>();
            serviceCollection.AddSingleton<INavUpdater, NavUpdater>();
            return serviceCollection;
        }
    }
}
