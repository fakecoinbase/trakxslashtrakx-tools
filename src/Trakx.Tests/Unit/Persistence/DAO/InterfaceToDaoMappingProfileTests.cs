using AutoMapper;
using FluentAssertions;
using Trakx.Common.Core;
using Trakx.Persistence.DAO;
using Xunit;

namespace Trakx.Tests.Unit.Persistence.DAO
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
            var component = new ComponentDefinition("0xabcdef", "test", "abc", "gecko", 15);
            var componentDefinitionDao = _mapper.Map<ComponentDefinitionDao>(component);

            componentDefinitionDao.Address.Should().Be("0xabcdef");
            componentDefinitionDao.Name.Should().Be("test");
            componentDefinitionDao.Symbol.Should().Be("abc");
            componentDefinitionDao.Decimals.Should().Be(15);
        }
    }
}
