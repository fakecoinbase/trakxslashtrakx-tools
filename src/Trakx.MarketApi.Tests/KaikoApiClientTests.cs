using System;
using System.Threading.Tasks;
using Trakx.MarketApi.DataSources.Kaiko;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.MarketApi.Tests
{
    public class KaikoApiClientTests
    {
        private readonly ITestOutputHelper _output;

        public KaikoApiClientTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetExchanges_should_return_exchanges()
        {
            var kaikoApi = new KaikoApiClient();
            var exchanges = await kaikoApi.GetExchanges();
            
            _output.WriteLine(exchanges.ToString());

            exchanges.Data.ForEach(e => _output.WriteLine($"{e.Code}, {e.Name}"));
        }

        [Fact]
        public async Task GetAssets_should_return_assets()
        {
            var kaikoApi = new KaikoApiClient();
            var assets = await kaikoApi.GetAssets();

            _output.WriteLine(assets.ToString());

            assets.Data.ForEach(a => _output.WriteLine($"{a.Code}, {a.Name}, {a.AssetClass}"));
        }


        [Fact]
        public async Task GetInstruments_should_return_instruments()
        {
            var kaikoApi = new KaikoApiClient();
            var instruments = await kaikoApi.GetInstruments();

            _output.WriteLine(instruments.ToString());

            instruments.Data.ForEach(i => _output.WriteLine($"{i.Code}, {i.ExchangeCode}, {i.ExchangePairCode}, {i.Class}, {i.BaseAsset}, {i.QuoteAsset}"));
        }
    }
}
