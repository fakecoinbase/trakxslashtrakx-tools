using AutoMapper.Configuration;
using Trakx.Common.Core;

namespace Trakx.Persistence.DAO
{
    public class InterfaceToDaoMappingProfile : MapperConfigurationExpression
    {
        /// <summary>
        /// Create automap mapping profiles
        /// </summary>
        public InterfaceToDaoMappingProfile()
        {
            CreateMap<ComponentDefinition, ComponentDefinitionDao>();

            CreateMap<ComponentWeight, ComponentWeightDao>()
                .ForMember(dest => dest.ComponentDefinitionDao, 
                    opt => opt.MapFrom(src => src.ComponentDefinition));

            CreateMap<ComponentQuantity, ComponentQuantityDao>()
                .ForMember(dest => dest.ComponentDefinitionDao,
                    opt => opt.MapFrom(src => src.ComponentDefinition));
            CreateMap<ComponentValuation, ComponentValuationDao>();
            CreateMap<IndiceComposition, IndiceCompositionDao>()
                .ForMember(dest => dest.ComponentQuantityDaos,
                    opt => opt.MapFrom(src => src.ComponentQuantities))
                .ForMember(dest => dest.IndiceDefinitionDao,
                    opt => opt.MapFrom(src => src.IndiceDefinition));
            CreateMap<IndiceValuation, IndiceValuationDao>()
                .ForMember(dest => dest.ComponentValuationDaos,
                    opt => opt.MapFrom(src => src.ComponentValuations));
        }
    }
}
