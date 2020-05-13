using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Persistence;
using Trakx.Persistence.DAO;
using Trakx.Tests.Unit.Models;
using Xunit;

namespace Trakx.Tests.Unit.Persistence
{
    [Collection(nameof(EmptyDbContextCollection))]
    public sealed class IndiceDataProviderTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IndiceDataProvider _indiceDataProvider;

        public IndiceDataProviderTests(EmptyDbContextFixture fixture)
        {
            _context = fixture.Context;
            _indiceDataProvider = new IndiceDataProvider(_context, Substitute.For<IMemoryCache>(), Substitute.For<ILogger<IndiceDataProvider>>());
        }


        [Fact]
        public async Task GetAllIndiceSymbols_should_return_all_indice_symbols()
        {
            var symbols = await _indiceDataProvider.GetAllIndiceSymbols(CancellationToken.None);
            symbols.Count.Should().NotBe(0);
        }

        [Fact(Skip = "impacts other tests negatively when running concurrently")]
        public async Task GetAllIndices_should_return_empty_if_database_empty()
        {
            _context.IndiceDefinitions.RemoveRange(_context.IndiceDefinitions);
            _context.SaveChanges();

            var indices = await _indiceDataProvider.GetAllIndices();
            indices.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllIndices_should_send_listOfIndices_if_all_is_ok()
        {
            var indices = await _indiceDataProvider.GetAllIndices();
            indices.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task TryToGetIndiceByAddress_should_return_true_if_indice_is_in_database()
        {
            var existingIndice = new IndiceDefinitionDao("test3.0", "Asset Management",
                "Indice composed of tokens from the Messari Asset Management sector",
                10,
                "0x7b0ef33d7d91f4d0f7e49e72fbe50d27522cf857", DateTime.Now);
            await _context.IndiceDefinitions.AddAsync(existingIndice);
            await _context.SaveChangesAsync();

            var result = await _indiceDataProvider.TryToGetIndiceByAddress(existingIndice.Address);

            result.Should().Be(true);
        }

        [Fact]
        public async Task TryToGetIndiceByAddress_should_return_false_if_indice_not_in_database()
        {
            var fakeIndice = new IndiceDefinitionDao("l1amg", "Asset Management",
                "Indice composed of tokens from the Messari Asset Management sector",
                10,
                "fake ethereum address", DateTime.Now);

            var result = await _indiceDataProvider.TryToGetIndiceByAddress(fakeIndice.Address);

            result.Should().Be(false);
        }

        [Fact]
        public async Task TryToGetCompositionByAddress_should_return_true_if_composition_is_in_database()
        {
            var symbol = "TRXc"; 
            var composition = new IndiceCompositionDao(new IndiceDefinitionDao(symbol, "NamePublished", null, 8, "0xdc6e10fbacf109efb74e0864cdce4876c7e729bf", null), 2, DateTime.Now, "existing address", null);


            await _context.IndiceCompositions.AddAsync(composition);
            await _context.SaveChangesAsync();

            var result = await _indiceDataProvider.TryToGetCompositionByAddress(composition.Address);

            result.Should().Be(true);
        }

        [Fact]
        public async Task TryToGetCompositionByAddress_should_return_false_if_composition_not_in_database()
        {
            var fakeAddress = "this is a fake";
            var result = await _indiceDataProvider.TryToGetCompositionByAddress(fakeAddress);

            result.Should().Be(false);
        }

        [Fact]
        public async Task GetAllCompositionForIndice_should_send_null_if_indice_not_in_database()
        {
            var fakeIndiceSymbol = "UnderTest";
            var result = await _indiceDataProvider.GetAllCompositionForIndice(fakeIndiceSymbol);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllCompositionForIndice_should_return_empty_list_if_indice_has_no_compositions()
        {
            var symbol = "TRXa";
            var indice =new IndiceDefinitionDao(symbol, null, "new description ", 8, "ethereum address", null);
            await _context.IndiceDefinitions.AddAsync(indice);
            await _context.SaveChangesAsync();

            var indiceSymbol = symbol;
            var result = await _indiceDataProvider.GetAllCompositionForIndice(indiceSymbol);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCompositionForIndice_should_return_listOfIndiceCompositions()
        {
            var symbol = "TRXb";
            var composition = new IndiceCompositionDao(new IndiceDefinitionDao(symbol, null, "new description ", 8, "ethereum address", null), 2, new DateTime(2006, 10, 20), "new address", null);
            await _context.IndiceCompositions.AddAsync(composition);
            await _context.SaveChangesAsync();

            var indiceSymbol = symbol;
            var result = await _indiceDataProvider.GetAllCompositionForIndice(indiceSymbol);
            result.Should().NotBeNullOrEmpty();
        }
    }
}
