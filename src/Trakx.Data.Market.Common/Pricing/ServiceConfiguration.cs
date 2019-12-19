using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Pricing
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddPricing(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<INavCalculator, NavCalculator>();
            serviceCollection.AddSingleton<INavUpdater, NavUpdater>();
            return serviceCollection;
        }
    }
}
