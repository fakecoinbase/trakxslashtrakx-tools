using System.Linq;

using FluentAssertions;

using Trakx.MarketData.Feeds.Models.CryptoCompare;

using Xunit;

namespace Trakx.MarketData.Feeds.Tests.CryptoCompare
{
    public class ParserTests
    {
        private TestData.CryptoCompare _dataProvider;

        public ParserTests()
        {
           
        }

        [Fact]
        public void ReadCoinList_Should_Parse_Response_Content()
        {
            var coinlistAsString = TestData.CryptoCompare.CoinListAsString.Value;

            var coinBySymbol = CryptoCompareResponse.FromJson(coinlistAsString);

            coinBySymbol.Data.Should().HaveCount(3233);
            coinBySymbol.Data.Keys.Should().NotContainNulls();
            coinBySymbol.Data.Values.Should().NotContainNulls();

            coinBySymbol.Data.ContainsKey("BTC").Should().BeTrue();
            var coin = coinBySymbol.Data["BTC"];
            coin.Id.Should().Be(1182);
            coin.Url.Should().Be(@"/coins/btc/overview");
            coin.ImageUrl.Should().Be(@"/media/19633/btc.png");
            coin.Name.Should().Be("BTC");
            coin.Symbol.Should().Be("BTC");
            coin.CoinName.Should().Be("Bitcoin");
            coin.FullName.Should().Be("Bitcoin (BTC)");
            coin.Algorithm.Should().Be("SHA256");
            coin.ProofType.Should().Be("PoW");
            //coin.FullyPremined.Should().Be("0");
            //coin.TotalCoinSupply.Should().Be(21000000);
            coin.BuiltOn.Should().BeNull();
            coin.SmartContractAddress.Should().Be("N/A");
            coin.PreMinedValue.Should().Be(null);
            coin.TotalCoinsFreeFloat.Should().Be(null);
            coin.SortOrder.Should().Be(1);
            coin.Sponsored.Should().BeFalse();
        }

        public void GetTop10Coins_Should_Return_Top_10_Coins()
        {

        }
    }
}
