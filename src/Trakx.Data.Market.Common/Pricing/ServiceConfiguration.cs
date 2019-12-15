using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Common.Pricing
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddPricing(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IIndexDefinitionProvider, IndexDefinitionProvider>();
            serviceCollection.AddScoped<INavCalculator, NavCalculator>();
            serviceCollection.AddScoped<INavUpdater, NavUpdater>();
            return serviceCollection;
        }
    }
}
