using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Persistence;
using Trakx.Tests.Unit.Models;
using Xunit;

namespace Trakx.Tests.Unit.Persistence
{
    [Collection(nameof(TestDbContextCollection))]
    public class IndexDataProviderTests
    {
        private readonly IndexRepositoryContext _context;
        private readonly IndexDataProvider _indexDataProvider;
        private readonly IMemoryCache _memoryCache;

        public IndexDataProviderTests(TestDbContextFixture fixture)
        {
            _context = fixture.Context;
            _memoryCache = Substitute.For<IMemoryCache>();
            _indexDataProvider = new IndexDataProvider(_context, _memoryCache, Substitute.For<ILogger<IndexDataProvider>>());
        }

        [Fact]
        public async Task GetAllIndexSymbols_should_return_all_index_symbols()
        {
            var symbols = await _indexDataProvider.GetAllIndexSymbols(CancellationToken.None);
            symbols.Count.Should().Be(5);
            symbols.Should().BeEquivalentTo(new List<string> {"l1abc", "s3def", "l8ghi", "s13jkl", "l22mno"});
        }
    }
}
