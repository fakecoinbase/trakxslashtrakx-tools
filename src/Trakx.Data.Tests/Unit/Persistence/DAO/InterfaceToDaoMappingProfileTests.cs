using AutoMapper;
using FluentAssertions;
using Trakx.Data.Common.Core;
using Trakx.Data.Persistence.DAO;
using Xunit;

namespace Trakx.Data.Tests.Unit.Persistence.DAO
{
    public class InterfaceToDaoMappingProfileTests
    {
        private readonly IMapper _mapper;

        public InterfaceToDaoMappingProfileTests()
        {
            var automapperConfig = new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new InterfaceToDaoMappingProfile());
            });

            _mapper = automapperConfig.CreateMapper();
        }

        [Fact]
        public void ComponentDefinition_Should_Map_Correctly()
        {
            var component = new ComponentDefinition("0xabcdef", "test", "abc", 15);
            var componentDefinitionDao = _mapper.Map<ComponentDefinitionDao>(component);

            componentDefinitionDao.Address.Should().Be("0xabcdef");
            componentDefinitionDao.Name.Should().Be("test");
            componentDefinitionDao.Symbol.Should().Be("abc");
            componentDefinitionDao.Decimals.Should().Be(15);
        }
    }
}
