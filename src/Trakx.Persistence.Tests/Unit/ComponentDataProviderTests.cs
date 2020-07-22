using System.Threading.Tasks;
using FluentAssertions;
using Trakx.Common.Interfaces;
using Trakx.Common.Tests;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests.Unit
{
    [Collection(nameof(EmptyDbContextCollection))]
    public class ComponentDataProviderTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IComponentDataProvider _componentDataProvider;
        private readonly MockCreator _mockCreator;

        public ComponentDataProviderTests(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _mockCreator = new MockCreator(output);
            _context = fixture.Context;
            _componentDataProvider = new ComponentDataProvider(_context);
        }

        [Fact(Skip = "Cannot run with the other tests")]
        public async Task GetAllComponentsFromDatabase_should_return_empty_list_if_database_empty()
        {
            _context.ComponentDefinitions.RemoveRange(_context.ComponentDefinitions);
            await _context.SaveChangesAsync();

            var result = await _componentDataProvider.GetAllComponentsFromDatabase();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllComponentsFromDatabase_should_return_list_of_ComponentDefinition()
        {
            await _context.ComponentDefinitions.AddAsync(GetComponentDefinitionDao());
            await _context.ComponentDefinitions.AddAsync(GetComponentDefinitionDao());
            await _context.SaveChangesAsync();

            var result = await _componentDataProvider.GetAllComponentsFromDatabase();
            result.Should().NotBeEmpty();
            result.Count.Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task SearchComponentByAddress_should_return_null_if_component_doesnt_exist()
        {
            var result = await _componentDataProvider.GetComponentFromDatabaseByAddress(_mockCreator.GetRandomAddressEthereum());

            result.Should().BeNull();
        }

        [Fact]
        public async Task SearchComponentByAddress_should_return_componentDefinition()
        {
            var componentDefinitionDao = GetComponentDefinitionDao();
            await _context.ComponentDefinitions.AddAsync(componentDefinitionDao);
            await _context.SaveChangesAsync();

            var retrievedComponent = await _componentDataProvider.GetComponentFromDatabaseByAddress(componentDefinitionDao.Address);

            retrievedComponent.Symbol.Should().Be(componentDefinitionDao.Symbol);
            retrievedComponent.Address.Should().Be(componentDefinitionDao.Address);
        }

        private ComponentDefinitionDao GetComponentDefinitionDao()
        {
            var component = _mockCreator.GetComponentQuantity().ComponentDefinition;
            var componentDao = new ComponentDefinitionDao(component);
            return componentDao;
        }
    }
}
