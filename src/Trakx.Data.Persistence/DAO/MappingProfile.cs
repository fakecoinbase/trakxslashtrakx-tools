using AutoMapper.Configuration;
using Trakx.Data.Common.Core;

namespace Trakx.Data.Persistence.DAO
{
    public class DaoMappingProfile : MapperConfigurationExpression
    {
        /// <summary>
        /// Create automap mapping profiles
        /// </summary>
        public DaoMappingProfile()
        {
            CreateMap<ComponentDefinition, ComponentDefinitionDao>();
            CreateMap<ComponentWeight, ComponentWeightDao>()
                .ForMember(dest => dest.ComponentDefinitionDao, 
                    opt => opt.MapFrom(src => src.ComponentDefinition));
            CreateMap<IndexDefinition, IndexDefinitionDao>()
                .ForMember(dest => dest.ComponentWeightDaos, 
                    opt => opt.MapFrom(src => src.ComponentWeights));
                //.ForAllOtherMembers();
            CreateMap<ComponentQuantity, ComponentQuantityDao>();
            CreateMap<ComponentValuation, ComponentValuationDao>();
            CreateMap<IndexComposition, IndexCompositionDao>();
            CreateMap<IndexValuation, IndexValuationDao>();
        }
    }
}
