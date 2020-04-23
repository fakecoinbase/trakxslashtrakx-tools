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
    public class IndiceDataProviderTests
    {
        private readonly IndiceRepositoryContext _context;
        private readonly IndiceDataProvider _indiceDataProvider;
        private readonly IMemoryCache _memoryCache;

        public IndiceDataProviderTests(TestDbContextFixture fixture)
        {
            _context = fixture.Context;
            _memoryCache = Substitute.For<IMemoryCache>();
            _indiceDataProvider = new IndiceDataProvider(_context, _memoryCache, Substitute.For<ILogger<IndiceDataProvider>>());
        }

        [Fact]
        public async Task GetAllIndiceSymbols_should_return_all_indice_symbols()
        {
            var symbols = await _indiceDataProvider.GetAllIndiceSymbols(CancellationToken.None);
            symbols.Count.Should().Be(5);
            symbols.Should().BeEquivalentTo(new List<string> {"l1abc", "s3def", "l8ghi", "s13jkl", "l22mno"});
        }
    }
}
