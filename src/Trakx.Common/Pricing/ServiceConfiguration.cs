using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Interfaces.Pricing;

namespace Trakx.Common.Pricing
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddPricing(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<INavCalculator, NavCalculator>();
            serviceCollection.AddScoped<INavUpdater, NavUpdater>();
            return serviceCollection;
        }
    }
}
