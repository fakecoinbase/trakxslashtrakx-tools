using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Trakx.Common.Interfaces;
using Trakx.Common.Models;
using Trakx.IndiceManager.Server.Managers;
using Trakx.Persistence;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests;
using Trakx.Persistence.Tests.Model;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.IndiceManager.Server.Tests.Integration.Managers
{
    public sealed class IndiceDatabaseWriterTest : IDisposable
    {
        private readonly IIndiceDatabaseWriter _indiceWriter;
        private readonly IndiceCompositionModel _composition;
        private readonly IndiceDetailModel _indiceDetailModel;
        private readonly IIndiceDataCreator _indiceDataCreator;
        private readonly IIndiceDataModifier _indiceDataModifier;
        private readonly IndiceRepositoryContext _dbContext;
        private readonly MockCreator _mock;

        public IndiceDatabaseWriterTest(ITestOutputHelper output)
        {
            _dbContext = new EmptyDbContextFixture().Context;
            _indiceDataCreator = Substitute.For<IIndiceDataCreator>();
            _indiceDataModifier = Substitute.For<IIndiceDataModifier>();
            _indiceWriter = new IndiceDatabaseWriter(_dbContext, _indiceDataModifier, _indiceDataCreator);
            _indiceDetailModel = new IndiceDetailModel
            {
                Address = "0xdc6e10fbacf109efb74e0864cdce4876c7e729bf",
                CreationDate = new DateTime(2018, 02, 10),
                Description = "OK all is good",
                Name = "TrakxTest",
                NaturalUnit = 10,
                Symbol = "TRX"
            };
            _composition = new IndiceCompositionModel
            {
                CreationDate = new DateTime(2006, 10, 20),
                IndiceDetail = _indiceDetailModel,
                Version = 2,
                Components = new List<ComponentDetailModel> { new ComponentDetailModel { Quantity = 3.0m, Symbol = "btc", Address = "0xab6e10fbacf109e45646664cdce4876c7e729bf", Name = "bitcoin", CoinGeckoId = "bitcoin" } }
            };
            _mock = new MockCreator(output);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task TrySaveComposition_should_return_false_if_indiceDetail_is_null()
        {
            _composition.IndiceDetail = null;
            var result = await _indiceWriter.TrySaveComposition(_composition);
            result.Should().Be(false);
        }

        [Fact]
        public async Task TrySaveComposition_should_modify_saved_composition()
        {
            var savedComposition = new IndiceCompositionDao(new IndiceDefinitionDao("TRX", null, null, 8, "published address", null), 2, DateTime.Now, null);
            await _dbContext.IndiceCompositions.AddAsync(savedComposition);
            await _dbContext.SaveChangesAsync();

            await _indiceWriter.TrySaveComposition(_composition);

            await _indiceDataModifier.ReceivedWithAnyArgs(1).ModifyComposition(default);
            await _indiceDataCreator.DidNotReceiveWithAnyArgs().AddNewComposition(default);
        }

        [Fact]
        public async Task TrySaveComposition_should_not_modify_published_composition()
        {
            var publishedComposition = new IndiceCompositionDao(new IndiceDefinitionDao("TRX", "Trakx test", null, 8, "published address", null), 2, DateTime.Now, "published address");
            await _dbContext.IndiceCompositions.AddAsync(publishedComposition);
            await _dbContext.SaveChangesAsync();

            await _indiceWriter.TrySaveComposition(_composition);

            await _indiceDataModifier.DidNotReceiveWithAnyArgs().ModifyComposition(default);
            await _indiceDataCreator.DidNotReceiveWithAnyArgs().AddNewComposition(default);
        }

        [Fact]
        public async Task TrySaveComposition_should_add_new_composition()
        {
            var savedIndice = new IndiceDefinitionDao("TRX", "Trakx test", null, 8, "ethereum address", null);
            await _dbContext.IndiceDefinitions.AddAsync(savedIndice);
            await _dbContext.SaveChangesAsync();

            await _indiceWriter.TrySaveComposition(_composition);

            await _indiceDataCreator.ReceivedWithAnyArgs(1).AddNewComposition(default);
            await _indiceDataModifier.DidNotReceiveWithAnyArgs().ModifyComposition(default);
        }

        [Fact]
        public async Task TrySaveComposition_should_add_new_composition_on_new_indice()
        {
            _composition.IndiceDetail.Symbol = _mock.GetRandomIndiceSymbol();
            await _indiceWriter.TrySaveComposition(_composition);

            await _indiceDataCreator.ReceivedWithAnyArgs(1).AddNewComposition(default);
            await _indiceDataModifier.DidNotReceiveWithAnyArgs().ModifyComposition(default);
        }


        [Fact]
        public async Task TrySaveIndice_should_modify_saved_indice()
        {
            var savedIndice = new IndiceDefinitionDao("TRX", "Trakx test", null, 8, null, null);
            await _dbContext.IndiceDefinitions.AddAsync(savedIndice);
            await _dbContext.SaveChangesAsync();

            await _indiceWriter.TrySaveIndice(_indiceDetailModel);

            await _indiceDataModifier.ReceivedWithAnyArgs(1).ModifyIndice(default);
            await _indiceDataCreator.DidNotReceiveWithAnyArgs().AddNewIndice(default);
        }

        [Fact]
        public async Task TrySaveIndice_should_add_new_indice()
        {
            await _indiceWriter.TrySaveIndice(_indiceDetailModel);

            await _indiceDataModifier.DidNotReceiveWithAnyArgs().ModifyIndice(default);
            await _indiceDataCreator.ReceivedWithAnyArgs(1).AddNewIndice(default);
        }

        [Fact]
        public async Task TrySaveIndice_should_not_modify_published_indice()
        {
            var publishedIndice = new IndiceDefinitionDao("TRX", "Trakx test", null, 8, "published address", null);
            await _dbContext.IndiceDefinitions.AddAsync(publishedIndice);
            await _dbContext.SaveChangesAsync();

            var result = await _indiceWriter.TrySaveIndice(_indiceDetailModel);

            result.Should().Be(false);
        }
    }
}
