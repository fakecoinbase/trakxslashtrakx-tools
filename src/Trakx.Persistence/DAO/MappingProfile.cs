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
            CreateMap<IndexComposition, IndexCompositionDao>()
                .ForMember(dest => dest.ComponentQuantityDaos,
                    opt => opt.MapFrom(src => src.ComponentQuantities))
                .ForMember(dest => dest.IndexDefinitionDao,
                    opt => opt.MapFrom(src => src.IndexDefinition));
            CreateMap<IndexValuation, IndexValuationDao>()
                .ForMember(dest => dest.ComponentValuationDaos,
                    opt => opt.MapFrom(src => src.ComponentValuations));
        }
    }
}
