using Microsoft.Extensions.DependencyInjection;

namespace Trakx.MarketApi.Pricing
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddPricing(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<NavCalculator>();
            return serviceCollection;
        }
    }
}
