using System.Threading.Tasks;
using Trakx.MarketApi.DataSources.CoinGecko;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketApi.Tests
{
    public class CoinGeckoApiClientTests
    {
        private readonly ITestOutputHelper _output;

        public CoinGeckoApiClientTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetTopTrustedExchanges_should_return_exchanges()
        {
            var kaikoApi = new CoinGeckoApiClient();
            var exchanges = await kaikoApi.GetTopTrustedExchanges(10);

            _output.WriteLine(exchanges.ToString());

            exchanges.ForEach(e => _output.WriteLine($"{e.TrustScoreRank}, {e.TrustScore}, {e.Name}, {e.Id}"));
        }
    }
}
