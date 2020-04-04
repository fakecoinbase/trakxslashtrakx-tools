using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Persistence.DAO;

namespace Trakx.MarketData.Server.Models
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddMappings(this IServiceCollection serviceCollection)
        {
            var automapperConfig = new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new InterfaceToDaoMappingProfile());
            });

            var autoMapper = automapperConfig.CreateMapper();

            serviceCollection.AddSingleton(autoMapper);
            return serviceCollection;
        }
    }
}