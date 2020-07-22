using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Trakx.Persistence.Tests.Model;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Persistence.Tests.Tools
{
    [Collection(nameof(SeededDbContextCollection))]
    public sealed class RebalanceHelper : IDisposable

    {
        private readonly ITestOutputHelper _output;
        private readonly SeededInMemoryIndiceRepositoryContext _dbContext;
        private readonly FlurlClient _flurlClient;

        public RebalanceHelper(SeededDbContextFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _dbContext = fixture.Context;
            _flurlClient = new FlurlClient("https://marketdata.trakx.io");
        }

        [Fact(Skip = "not a test")]
        public void Print_rebalancing_Nav()
        {
            var currentSuffix = "2006";
            var nextSuffix = "2007";
            var symbols = _dbContext.IndiceDefinitions.ToList()
                .Select(i => i.Symbol);

            var navByComposition = symbols.ToDictionary(
                    s => s + nextSuffix,
                    s => GetCurrentNavForComposition(s + currentSuffix).GetAwaiter().GetResult());

            foreach (var pair in navByComposition.Where(p => p.Value != 0))
            {
                _output.WriteLine($"{{\"{pair.Key}\", {pair.Value}m}},");
            }
        }

        private async Task<decimal> GetCurrentNavForComposition(string compositionSymbol)
        {
            var response = await _flurlClient
                .Request("nav", "GetUsdNetAssetValue")
                .SetQueryParam("indiceOrCompositionSymbol", compositionSymbol)
                .SendAsync(HttpMethod.Get);
            if (!response.IsSuccessStatusCode) return 0;
            var responseBody = await response.Content.ReadAsStringAsync();
            return decimal.TryParse(responseBody, out var nav) ? nav : 0m;
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _dbContext?.Dispose();
            _flurlClient?.Dispose();
        }

        #endregion
    }
}
