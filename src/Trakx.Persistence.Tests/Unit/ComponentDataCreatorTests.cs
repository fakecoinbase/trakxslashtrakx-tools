using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests.Model;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests.Unit
{
    [Collection(nameof(EmptyDbContextCollection))]
    public class ComponentDataCreatorTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IComponentDataCreator _componentDataCreator;
        private readonly MockCreator _mockCreator;

        public ComponentDataCreatorTests(EmptyDbContextFixture fixture,ITestOutputHelper output)
        {
            _mockCreator=new MockCreator(output);
            _context = fixture.Context;
            _componentDataCreator = new ComponentDataCreator(_context);
        }

        [Fact]
        public async Task TryToSaveComponentDefinition_should_return_false_if_object_already_in_database()
        {
            var component = _mockCreator.GetComponentQuantity().ComponentDefinition;
            
            var componentDao = new ComponentDefinitionDao(component);

            await _context.ComponentDefinitions.AddAsync(componentDao);
            await _context.SaveChangesAsync();

            var result = await _componentDataCreator.TryAddComponentDefinition(component);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task TryToSaveComponentDefinition_should_return_true_if_addition_in_database_succeed()
        {
            var component = _mockCreator.GetComponentQuantity().ComponentDefinition;

            var result = await _componentDataCreator.TryAddComponentDefinition(component);
            result.Should().BeTrue();

            var retrievedComponent = await 
                _context.ComponentDefinitions.FirstOrDefaultAsync(c => c.Address == component.Address);

            retrievedComponent.Address.Should().Be(component.Address);
            retrievedComponent.CoinGeckoId.Should().Be(component.CoinGeckoId);
            retrievedComponent.Name.Should().Be(component.Name);
        }
    }
}
