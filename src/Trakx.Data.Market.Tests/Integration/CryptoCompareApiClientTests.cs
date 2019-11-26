//using System.Threading.Tasks;
//using FluentAssertions;
//using Trakx.Data.Market.Common.Sources.CryptoCompare;
//using Xunit;
//using Xunit.Abstractions;

//namespace Trakx.Data.Market.Tests.Integration
//{
//    public class CryptoCompareApiClientTests
//    {
//        private readonly ITestOutputHelper _output;

//        public CryptoCompareApiClientTests(ITestOutputHelper output)
//        {
//            _output = output;
//        }

//        [Fact]
//        public async Task GetAllErc20Tokens_should_return_coins_with_smart_contract_addresses()
//        {
//            var client = new CryptoCompareApiClient();
//            var erc20s = client.GetAllErc20Symbols();
            
//            erc20s.Count.Should().BeGreaterThan(10);

//            erc20s.ForEach(e => _output.WriteLine(e));
//        }
//    }
//}
