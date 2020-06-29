using System;
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
    public sealed class IndiceDataCreatorTests
    {
        private const string IndiceName = "TrakxTest";
        private const string IndiceDescription = "nothing to describe";
        private const int NaturalUnit = 10;

        private readonly IndiceRepositoryContext _context;
        private readonly IIndiceDataCreator _indiceDataCreator;
        private readonly string _indiceSymbol;

        public IndiceDataCreatorTests(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _indiceDataCreator = new IndiceDataCreator(_context);
            var mockCreator = new MockCreator(output);
            _indiceSymbol = mockCreator.GetRandomIndiceSymbol();
        }

        [Fact]
        public async Task AddIndice_should_return_true_if_indice_is_added()
        {
            var indiceToAdd = new IndiceDefinitionDao(_indiceSymbol, IndiceName, IndiceDescription, NaturalUnit, null, DateTime.Now);

            var result = await _indiceDataCreator.AddNewIndice(indiceToAdd);

            var newIndice = await _context.IndiceDefinitions.FirstOrDefaultAsync(i => i.Symbol == indiceToAdd.Symbol);

            result.Should().Be(true);
            newIndice.Description.Should().Be(indiceToAdd.Description);
            newIndice.Name.Should().Be(indiceToAdd.Name);
        }

        [Fact]
        public async Task AddComposition_should_return_true_if_composition_is_added_on_existing_indice()
        {
            var savedIndice = new IndiceDefinitionDao(_indiceSymbol, IndiceName, IndiceDescription, NaturalUnit, null, DateTime.Now);
            var compositionToAdd = new IndiceCompositionDao(savedIndice, 3, DateTime.Now, null);
            compositionToAdd.ComponentQuantityDaos.Add(new ComponentQuantityDao(compositionToAdd, new ComponentDefinitionDao("address", "name", "symbol", "coinGeckoId", NaturalUnit), 3));

            var result = await _indiceDataCreator.AddNewComposition(compositionToAdd);

            var retrievedComposition = await _context.IndiceCompositions.FirstOrDefaultAsync(i => i.Id == compositionToAdd.Id);

            result.Should().Be(true);
            retrievedComposition.CheckCompositionIsAsExpected(compositionToAdd);
        }

        [Fact]
        public async Task AddComposition_should_return_true_if_composition_is_added_on_new_indice()
        {
            var newIndice = new IndiceDefinitionDao(_indiceSymbol, IndiceName, IndiceDescription, NaturalUnit, null, DateTime.Now);
            
            var compositionToAdd = new IndiceCompositionDao(newIndice, 2, DateTime.Now, null);
            compositionToAdd.ComponentQuantityDaos.Add(new ComponentQuantityDao(compositionToAdd, new ComponentDefinitionDao("ComponentAddress", "name", "symbol", "coinGeckoId", NaturalUnit), 3));

            var result =
                await _indiceDataCreator.AddNewComposition(compositionToAdd);

            var retrievedComposition = await _context.IndiceCompositions.FirstOrDefaultAsync(i => i.Id == compositionToAdd.Id);

            result.Should().Be(true);
            
            retrievedComposition.CheckCompositionIsAsExpected(compositionToAdd);
        }
    }

    public static class CompositionCheckExtensions
    {
        public static void CheckCompositionIsAsExpected(this IndiceCompositionDao retrievedComposition,
            IndiceCompositionDao expectedComposition)
        {
            retrievedComposition.CreationDate.Should().Be(expectedComposition.CreationDate);

            retrievedComposition.IndiceDefinitionDao.Name.Should().Be(expectedComposition.IndiceDefinition.Name);
            retrievedComposition.IndiceDefinitionDao.NaturalUnit.Should().Be(expectedComposition.IndiceDefinition.NaturalUnit);

            retrievedComposition.ComponentQuantityDaos.Count.Should().Be(1);
            retrievedComposition.ComponentQuantityDaos[0].Quantity.Should()
                .Be(expectedComposition.ComponentQuantityDaos[0].Quantity);
            retrievedComposition.ComponentQuantityDaos[0].ComponentDefinitionDao.Symbol.Should()
                .Be(expectedComposition.ComponentQuantityDaos[0].ComponentDefinitionDao.Symbol);
        }
    }
}
