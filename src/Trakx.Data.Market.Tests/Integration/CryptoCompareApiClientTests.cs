using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoCompare;
using Trakx.Data.Market.Common.Sources.CryptoCompare;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Market.Tests.Integration
{
    public class CryptoCompareApiClientTests
    {
        private readonly ITestOutputHelper _output;
        private readonly CryptoCompareClient _client;

        public CryptoCompareApiClientTests(ITestOutputHelper output)
        {
            _output = output;
            _client = new CryptoCompareClient(Constants.ApiKey);
        }

        [Fact]
        public async Task GetAllErc20Tokens_should_return_coins_with_smart_contract_addresses()
        {
            var client = new CryptoCompareClient();
            
        }

        [Fact]
        public async Task Can_get_simple_prices()
        {
            var price = await _client.Prices.SingleSymbolPriceAsync("BTC", new[] {"USD"});
            _output.WriteLine(price["USD"].ToString());
        }
    }
}
