using System.Linq;
using FluentAssertions;
using Trakx.Common.Interfaces.Indice;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Tests.Unit.Models.Indice
{
    [Collection(nameof(SeededDbContextCollection))]
    public class IndiceDefinitionTests
    {
        private readonly SeededDbContextFixture _fixture;
        private readonly ITestOutputHelper _output;
        private readonly int _expectedIndiceCount;
        private readonly int _expectedComponentCount;
        private readonly int _expectedCompositionCount;
        private readonly int _expectedQuantitiesCount;

        public IndiceDefinitionTests(SeededDbContextFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
            _expectedIndiceCount = 10;
            _expectedCompositionCount = 23 + 7 + 8;
            _expectedComponentCount = 63;
            _expectedQuantitiesCount = 52 + 53 + 25 + 28 + 28 + 32;
        }

        [Fact]
        public void ComponentDefinitions_should_not_be_empty()
        {
            var componentDefinitions = _fixture.Context.ComponentDefinitions;
            componentDefinitions.Count().Should().Be(_expectedComponentCount);

            foreach (var componentDefinitionDao in componentDefinitions)
            {
                componentDefinitionDao.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void IndiceDefinitions_should_not_be_empty()
        {
            var indiceDefinitions = _fixture.Context.IndiceDefinitions;
            indiceDefinitions.Count().Should().Be(_expectedIndiceCount);
            foreach (var indiceDefinitionDao in indiceDefinitions)
            {
                indiceDefinitionDao.IsValid().Should().BeTrue();
            }
        }
        
        [Fact]
        public void ComponentQuantities_should_not_be_empty()
        {
            var components = _fixture.Context.ComponentQuantities;
            components.Count().Should().Be(_expectedQuantitiesCount);
            foreach (var component in components)
            {
                component.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void IndiceCompositions_should_not_be_empty()
        {
            var indiceCompositions = _fixture.Context.IndiceCompositions.ToList();
            indiceCompositions.OrderBy(c => c.Symbol).ThenBy(c => c.Version).ToList()
                .ForEach(s => _output.WriteLine($"{s.Symbol} - v{s.Version}"));
            indiceCompositions.Count.Should().Be(_expectedCompositionCount);

            foreach (var indiceCompositionDao in indiceCompositions)
            {
                indiceCompositionDao.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void ComponentValuations_should_not_be_empty()
        {
            var componentValuations = _fixture.Context.ComponentValuations;
            componentValuations.Count().Should().Be(_expectedQuantitiesCount);
            foreach (var componentValuation in componentValuations)
            {
                componentValuation.IsValid().Should().BeTrue();
                componentValuation.Weight.HasValue.Should().BeTrue();
            }
        }


        [Fact]
        public void IndiceValuations_should_not_be_empty()
        {
            var indiceValuations = _fixture.Context.IndiceValuations;
            indiceValuations.Count().Should().Be(_expectedCompositionCount);

            foreach (var valuation in indiceValuations)
            {
                valuation.IsValid().Should().BeTrue();
            }
        }
    }
}