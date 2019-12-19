using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Models.Index;


namespace Trakx.Data.Market.Common.Serialisation
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddMappings(this IServiceCollection serviceCollection)
        {
            var automapperConfig = new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new MappingProfile());
            });

            var autoMapper = automapperConfig.CreateMapper();

            serviceCollection.AddSingleton(autoMapper);
            return serviceCollection;
        }
    }

    public class MappingProfile : MapperConfigurationExpression
    {
        /// <summary>
        /// Create automap mapping profiles
        /// </summary>
        public MappingProfile()
        {
            CreateMap<IndexDefinition, IndexDefinitionDto>().ReverseMap();
        }
    }

    public class IndexDefinitionDto
    {

    }
}
