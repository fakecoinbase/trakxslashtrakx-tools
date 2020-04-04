using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Interfaces.Pricing;

namespace Trakx.Common.Pricing
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
