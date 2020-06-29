using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Trakx.Common.Interfaces;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests.Model;
using Xunit;

namespace Trakx.Persistence.Tests.Unit
{
    [Collection(nameof(EmptyDbContextCollection))]
    public sealed class IndiceDataModifierTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IIndiceDataModifier _indiceDataModifier;

        public IndiceDataModifierTests(EmptyDbContextFixture fixture)
        {
            _context = fixture.Context;
            _indiceDataModifier = new IndiceDataModifier(_context);
        }

        [Fact]
        public async Task ModifyIndice_should_return_true_if_indice_is_modified()
        {
            var savedIndice = new IndiceDefinitionDao("test2.0","TrakxTest","nothing to describe",10,null,DateTime.Now);
            await _context.IndiceDefinitions.AddAsync(savedIndice);
            await _context.SaveChangesAsync();

            var modifyIndice = new IndiceDefinitionDao("test2.0", "New name here", "New description here", 10, "Published address", DateTime.Now);

            var result = await _indiceDataModifier.ModifyIndice(modifyIndice);
            var newIndice = await _context.IndiceDefinitions.FirstOrDefaultAsync(i => i.Symbol == savedIndice.Symbol);
            result.Should().Be(true);
            newIndice.Description.Should().Be(modifyIndice.Description);
            newIndice.Name.Should().Be(modifyIndice.Name);
        }

        [Fact]
        public async Task ModifyComposition_should_return_true_and_modify_composition_and_indice()
        {
            var firstComposition = new IndiceCompositionDao(new IndiceDefinitionDao("TRX", null, null, 8, null, null), 2, DateTime.Now, null);
            await _context.IndiceCompositions.AddAsync(firstComposition);
            await _context.SaveChangesAsync();

            var modifiedComposition = new IndiceCompositionDao(new IndiceDefinitionDao("TRX", null, "new description ", 8, "ethereum address", null), 2, new DateTime(2006, 10, 20), "new address");
            modifiedComposition.ComponentQuantityDaos.Add(new ComponentQuantityDao(modifiedComposition,new ComponentDefinitionDao("addressXX","name","symbol","coinGeckoId",10),3));
            
            var result = await _indiceDataModifier.ModifyComposition(modifiedComposition);
            var retrievedComposition = await _context.IndiceCompositions.FirstOrDefaultAsync(i => i.Id == firstComposition.Id);

            result.Should().Be(true);

            retrievedComposition.CheckCompositionIsAsExpected(modifiedComposition);
        }
    }
}
