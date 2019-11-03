using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Data.Market.Common.Pricing
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
