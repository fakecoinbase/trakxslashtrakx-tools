using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Persistence.DAO;
using Trakx.Persistence.Tests.Model;
using Trakx.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests.Unit
{
    [Collection(nameof(EmptyDbContextCollection))]
    public sealed class IndiceDataProviderTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IndiceDataProvider _indiceDataProvider;
        private readonly MockDaoCreator _mockDaoCreator;
        private readonly IMemoryCache _memoryCache;

        public IndiceDataProviderTests(EmptyDbContextFixture fixture, ITestOutputHelper output)
        {
            _context = fixture.Context;
            _memoryCache = Substitute.For<IMemoryCache>();
            _indiceDataProvider = new IndiceDataProvider(_context, _memoryCache, Substitute.For<ILogger<IndiceDataProvider>>());
            _mockDaoCreator = new MockDaoCreator(output);
        }

        [Fact]
        public async Task GetAllIndices_should_return_listOfIndices_if_all_is_ok()
        {
            var indices = new[]
            {
                _mockDaoCreator.GetRandomIndiceDefinitionDao(),
                _mockDaoCreator.GetRandomIndiceDefinitionDao()
            };

            await _context.IndiceDefinitions.AddRangeAsync(indices).ConfigureAwait(false);
            await _context.SaveChangesAsync();

            var retrievedIndices = await _indiceDataProvider.GetAllIndices();
            retrievedIndices.Should().NotBeNullOrEmpty();
            retrievedIndices.Should().Contain(indices);
        }

        [Fact]
        public async Task TryToGetIndiceByAddress_should_return_true_if_indice_is_in_database()
        {
            var existingIndice = await PersistRandomIndice();

            var result = await _indiceDataProvider.TryToGetIndiceByAddress(existingIndice.Address);

            result.Should().Be(true);
        }

        [Fact]
        public async Task TryToGetIndiceByAddress_should_return_false_if_indice_not_in_database()
        {
            var unknownIndice = _mockDaoCreator.GetRandomIndiceDefinitionDao();

            var result = await _indiceDataProvider.TryToGetIndiceByAddress(unknownIndice.Address);

            result.Should().Be(false);
        }

        [Fact]
        public async Task TryToGetCompositionByAddress_should_return_true_if_composition_is_in_database()
        {
            var composition = await PersistRandomComposition();

            var result = await _indiceDataProvider.TryToGetCompositionByAddress(composition.Address);

            result.Should().Be(true);
        }

        [Fact]
        public async Task TryToGetCompositionByAddress_should_return_false_if_composition_not_in_database()
        {
            var unknownAddress = _mockDaoCreator.GetRandomAddressEthereum();
            var result = await _indiceDataProvider.TryToGetCompositionByAddress(unknownAddress);

            result.Should().Be(false);
        }

        [Fact]
        public async Task GetAllCompositionForIndice_should_return_null_if_indice_not_in_database()
        {
            var unknownIndiceSymbol = _mockDaoCreator.GetRandomIndiceSymbol();
            var result = await _indiceDataProvider.GetAllCompositionForIndice(unknownIndiceSymbol);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllCompositionForIndice_should_return_empty_list_if_indice_has_no_compositions()
        {
            var indice = await PersistRandomIndice();

            var result = await _indiceDataProvider.GetAllCompositionForIndice(indice.Symbol);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCompositionForIndice_should_return_listOfIndiceCompositions()
        {
            var composition = await PersistRandomComposition();

            var result = await _indiceDataProvider.GetAllCompositionForIndice(composition.IndiceDefinition.Symbol);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetDefinitionFromSymbol_should_retrieve_index_definition_by_symbol()
        {
            var definition = await PersistRandomIndice().ConfigureAwait(false);

            var result = await _indiceDataProvider.GetDefinitionFromSymbol(definition.Symbol)
                .ConfigureAwait(false);

            result.Should().NotBeNull();
            result!.Symbol.Should().Be(definition.Symbol);
            result!.Address.Should().Be(definition.Address);
        }

        [Fact]
        public async Task GetDefinitionFromSymbol_should_return_null_on_unknown_defintion_symbol()
        {
            var definitionSymbol = _mockDaoCreator.GetRandomIndiceSymbol();

            var result = await _indiceDataProvider.GetDefinitionFromSymbol(definitionSymbol)
                .ConfigureAwait(false);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCompositionAtDate_should_save_to_memory_cache_on_retrieved_valid_data()
        {
            var composition = await PersistRandomComposition();
            var cacheEntry = SetUpCacheExpectation();
            
            var retrieved = await _indiceDataProvider.GetCompositionAtDate(composition.IndiceDefinition.Symbol, composition.CreationDate);

            retrieved!.Symbol.Should().Be(composition.Symbol);

            CheckUnknownEntryWasCreated();

            cacheEntry.Key.ToString().Should().Contain(composition.IndiceDefinition.Symbol);
            cacheEntry.Key.ToString().Should().Contain(composition.CreationDate.ToString("yyMMddHHmm"));
            ((IndiceCompositionDao)cacheEntry.Value).Id.Should().Be(composition.Id);
            
            cacheEntry.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task GetCompositionAtDate_should_expire_immediately_on_retrieving_invalid_data()
        {
            var cacheEntry = SetUpCacheExpectation();

            var retrieved = await _indiceDataProvider.GetCompositionAtDate(_mockDaoCreator.GetRandomIndiceSymbol(), 
                _mockDaoCreator.GetRandomDateTime());

            CheckInvalidEntryHasImmediateExpiration(retrieved, cacheEntry);
        }

        [Fact]
        public async Task GetCompositionFromSymbol_should_save_to_memory_cache_on_retrieved_valid_data()
        {
            var composition = await PersistRandomComposition();
            var cacheEntry = SetUpCacheExpectation();

            var retrieved = await _indiceDataProvider.GetCompositionFromSymbol(composition.Symbol);

            retrieved!.Symbol.Should().Be(composition.Symbol);
            
            CheckUnknownEntryWasCreated();

            cacheEntry.Key.ToString().Should().Contain(composition.Symbol);
            ((IndiceCompositionDao)cacheEntry.Value).Id.Should().Be(composition.Id);
            
            cacheEntry.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromDays(1));
        }

        [Fact]
        public async Task GetCompositionFromSymbol_should_expire_immediately_on_retrieving_invalid_data()
        {
            var cacheEntry = SetUpCacheExpectation();

            var retrieved = await _indiceDataProvider.GetCompositionFromSymbol(_mockDaoCreator.GetRandomCompositionSymbol());

            CheckInvalidEntryHasImmediateExpiration(retrieved, cacheEntry);
        }


        [Fact]
        public async Task GetInitialValuation_should_save_to_memory_cache_on_retrieved_valid_data()
        {
            var valuation = await PersistRandomInitialIndiceValuation();
            var cacheEntry = SetUpCacheExpectation();

            var quoteCurrency = "usdc";
            var retrieved = await _indiceDataProvider.GetInitialValuation(valuation.IndiceComposition, quoteCurrency);

            retrieved!.NetAssetValue.Should().Be(valuation.NetAssetValue);

            CheckUnknownEntryWasCreated();

            cacheEntry.Key.ToString().Should().Contain(valuation.IndiceComposition.Symbol);
            cacheEntry.Key.ToString().Should().Contain(quoteCurrency);
            ((IndiceValuationDao)cacheEntry.Value).Id.Should().Be(valuation.Id);

            cacheEntry.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromDays(1));
        }

        [Fact]
        public async Task GetInitialValuation_should_expire_immediately_on_retrieving_invalid_data()
        {
            var cacheEntry = SetUpCacheExpectation();

            var retrieved = await _indiceDataProvider.GetInitialValuation(_mockDaoCreator.GetRandomCompositionDao());

            CheckInvalidEntryHasImmediateExpiration(retrieved, cacheEntry);
        }

        private static void CheckInvalidEntryHasImmediateExpiration(object retrieved, ICacheEntry cacheEntry)
        {
            retrieved.Should().BeNull();
            cacheEntry.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromTicks(1));
        }

        private void CheckUnknownEntryWasCreated()
        {
            _memoryCache.Received(1).TryGetValue(Arg.Any<object>(), out Arg.Any<object>());
            _memoryCache.ReceivedWithAnyArgs(1).CreateEntry(default);
        }

        private async Task<IndiceValuationDao> PersistRandomInitialIndiceValuation()
        {
            var valuation = _mockDaoCreator.GetRandomIndiceValuationDao(true);
            await _context.IndiceValuations.AddAsync(valuation);
            await _context.SaveChangesAsync();
            return valuation;
        }
        private async Task<IndiceCompositionDao> PersistRandomComposition()
        {
            var composition = _mockDaoCreator.GetRandomCompositionDao();
            await _context.IndiceCompositions.AddAsync(composition);
            await _context.SaveChangesAsync();
            return composition;
        }

        private async Task<IndiceDefinitionDao> PersistRandomIndice()
        {
            var indice = _mockDaoCreator.GetRandomIndiceDefinitionDao();
            await _context.IndiceDefinitions.AddAsync(indice);
            await _context.SaveChangesAsync();
            return indice;
        }

        private ICacheEntry SetUpCacheExpectation()
        {
            var cacheEntry = Substitute.For<ICacheEntry>();
            _memoryCache.CreateEntry(Arg.Any<object>())
                .ReturnsForAnyArgs(ci =>
            {
                cacheEntry.Key.Returns(ci[0].ToString());
                return cacheEntry;
            });
            return cacheEntry;
        }
    }
}
