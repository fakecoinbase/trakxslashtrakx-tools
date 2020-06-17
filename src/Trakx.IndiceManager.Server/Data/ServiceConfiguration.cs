using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Interfaces;
using Trakx.IndiceManager.Server.Managers;
using Trakx.Persistence;
using Trakx.Persistence.DAO;

namespace Trakx.IndiceManager.Server.Data
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

        public static IServiceCollection AddAllManagerForControllers(this IServiceCollection service)
        {
            service.AddScoped<IComponentInformationRetriever, ComponentInformationRetriever>();
            service.AddScoped<IIndiceInformationRetriever, IndiceInformationRetriever>();
            service.AddScoped<IIndiceDatabaseWriter, IndiceDatabaseWriter>();
            service.AddScoped<IIndiceSupplyService, IndiceSupplyService>();

            return service;
        }

        public static IServiceCollection AddDatabaseFunctions(this IServiceCollection service)
        {
            service.AddScoped<IIndiceDataProvider, IndiceDataProvider>();
            service.AddScoped<IIndiceDataModifier, IndiceDataModifier>();
            service.AddScoped<IIndiceDataCreator, IndiceDataCreator>();
            service.AddScoped<ITransactionDataProvider, TransactionDataProvider>();
            service.AddScoped<ITransactionDataCreator, TransactionDataCreator>();
            service.AddScoped<ITransactionDataModifier, TransactionDataModifier>();
            service.AddScoped<IComponentInformationRetriever, ComponentInformationRetriever>();
            service.AddScoped<IComponentDataCreator, ComponentDataCreator>();
            service.AddScoped<IComponentDataProvider, ComponentDataProvider>();
            service.AddScoped<IUserAddressProvider, UserAddressProvider>();
            return service;
        } 

    }
}