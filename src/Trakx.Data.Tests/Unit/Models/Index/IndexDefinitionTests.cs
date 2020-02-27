using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Market.Server.Models;
using Xunit;

namespace Trakx.Data.Tests.Unit.Models.Index
{
    public sealed class DbContextFixture : IDisposable
    {
        public TestIndexRepositoryContext Context { get; private set; }
        

        public DbContextFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMappings();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            Context = new TestIndexRepositoryContext(mapper);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Context?.Dispose();
        }

        #endregion
    }

    public class IndexDefinitionTests : IClassFixture<DbContextFixture>
    {
        private readonly DbContextFixture _fixture;
        private int _expectedIndexCounts = 5;
        private int _expectedComponentCount = 54;

        public IndexDefinitionTests(DbContextFixture fixture)
        {
            _fixture = fixture;
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
        public void IndexDefinitions_should_not_be_empty()
        {
            var indexDefinitions = _fixture.Context.IndexDefinitions;
            indexDefinitions.Count().Should().Be(_expectedIndexCounts);
            foreach (var indexDefinitionDao in indexDefinitions)
            {
                indexDefinitionDao.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void ComponentWeights_should_not_be_empty()
        {
            var componentWeights = _fixture.Context.ComponentWeights;
            componentWeights.Count().Should().Be(_expectedComponentCount);
            foreach (var componentWeight in componentWeights)
            {
                componentWeight.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void ComponentQuantities_should_not_be_empty()
        {
            var components = _fixture.Context.ComponentQuantities;
            components.Count().Should().Be(_expectedComponentCount);
            foreach (var component in components)
            {
                component.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void IndexCompositions_should_not_be_empty()
        {
            var indexCompositions = _fixture.Context.IndexCompositions;
            indexCompositions.Count().Should().Be(_expectedIndexCounts);
            foreach (var indexCompositionDao in indexCompositions)
            {
                indexCompositionDao.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void ComponentValuations_should_not_be_empty()
        {
            var componentValuations = _fixture.Context.ComponentValuations;
            componentValuations.Count().Should().Be(_expectedComponentCount);
            foreach (var componentValuation in componentValuations)
            {
                componentValuation.IsValid().Should().BeTrue();
                componentValuation.Weight.HasValue.Should().BeTrue();
            }
        }


        [Fact]
        public void IndexValuations_should_not_be_empty()
        {
            var indexValuations = _fixture.Context.IndexValuations;
            indexValuations.Count().Should().Be(_expectedIndexCounts);

            foreach (var valuation in indexValuations)
            {
                valuation.IsValid().Should().BeTrue();
            }
        }
    }
}