using AutoMapper.Configuration;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Persistence.DAO
{
    public class InterfaceToDaoMappingProfile : MapperConfigurationExpression
    {
        /// <summary>
        /// Create automap mapping profiles
        /// </summary>
        public InterfaceToDaoMappingProfile()
        {
            CreateMap<IComponentDefinition, ComponentDefinitionDao>();

            CreateMap<IComponentWeight, ComponentWeightDao>()
                .ForMember(dest => dest.ComponentDefinitionDao, 
                    opt => opt.MapFrom(src => src.ComponentDefinition));

            CreateMap<IIndexDefinition, IndexDefinitionDao>()
                .ForMember(dest => dest.ComponentWeightDaos, 
                    opt => opt.MapFrom(src => src.ComponentWeights));

            CreateMap<IComponentQuantity, ComponentQuantityDao>()
                .ForMember(dest => dest.ComponentDefinitionDao,
                    opt => opt.MapFrom(src => src.ComponentDefinition));
            CreateMap<IComponentValuation, ComponentValuationDao>();
            CreateMap<IIndexComposition, IndexCompositionDao>()
                .ForMember(dest => dest.ComponentQuantityDaos,
                    opt => opt.MapFrom(src => src.ComponentQuantities))
                .ForMember(dest => dest.IndexDefinitionDao,
                    opt => opt.MapFrom(src => src.IndexDefinition));
            CreateMap<IIndexValuation, IndexValuationDao>()
                .ForMember(dest => dest.ComponentValuationDaos,
                    opt => opt.MapFrom(src => src.ComponentValuations));
        }
    }
}
