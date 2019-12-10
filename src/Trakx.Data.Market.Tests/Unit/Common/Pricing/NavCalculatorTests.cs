using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using CryptoCompare;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;
using Trakx.Data.Market.Common.Sources.Messari.Client;
using Trakx.Data.Market.Tests.Data.Kaiko;
using Trakx.Data.Market.Tests.Data.Messari;
using Xunit;

namespace Trakx.Data.Market.Tests.Unit.Common.Pricing
{
    public class NavCalculatorTests
    {
        private readonly NavCalculator _navCalculator;

        public NavCalculatorTests()
        {
            var indexProvider = PrepareIndexDetailsProvider();

            var aggregatedPriceReader = new KaikoReader();
            var kaikoClient = Substitute.For<IKaikoClient>();
            kaikoClient.GetSpotExchangeRate(Arg.Any<SpotExchangeRateRequest>())
                .Returns(async callInfo =>
                {
                    var symbol = ((SpotExchangeRateRequest)callInfo[0]).BaseAsset;
                    var prices = await aggregatedPriceReader.GetSpotExchangeRateForSymbol(symbol, false)
                        .ConfigureAwait(false);
                    return prices;
                });
            kaikoClient.CreateSpotExchangeRateRequest(Arg.Any<string>(), "usd")
                .Returns(ci => new SpotExchangeRateRequest() {BaseAsset = (string)ci[0]});

            var messariReader = new MessariReader();
            var messariClient = Substitute.For<IMessariClient>();
            messariClient.GetMetricsForSymbol(Arg.Any<string>())
                .Returns(async callInfo =>
                {
                    var symbol = (string)callInfo[0];
                    var price = await messariReader.GetAssetMetrics(symbol)
                        .ConfigureAwait(false);
                    return price;
                });

            var cryptoCompareClient = new CryptoCompareClient(new MockedCryptoCompareHttpHandler());

            var logger = Substitute.For<ILogger<NavCalculator>>();

            _navCalculator = new NavCalculator(kaikoClient, messariClient, cryptoCompareClient, indexProvider, logger);
        }

        private IIndexDetailsProvider PrepareIndexDetailsProvider()
        {
            var indexProvider = Substitute.For<IIndexDetailsProvider>();
            var indexDetails = Substitute.For<IDictionary<KnownIndexes, IndexDetails>>();
            indexProvider.IndexDetails.Returns(indexDetails);
            indexProvider.IndexDetails
                .TryGetValue(KnownIndexes.L1CPU003, out Arg.Any<IndexDetails>())
                .Returns(callInfo =>
                {
                    var maxDecimals = 18;
                    var minDecimals = 8;
                    var decimalDiff = maxDecimals - minDecimals;
                    callInfo[1] = new IndexDetails()
                    {
                        TargetUsdPrice = 1,
                        NaturalUnit = BigInteger.Pow(10, decimalDiff),
                        Components = new List<Component>()
                        {
                            new Component()
                            {
                                Symbol = "SYM1",
                                Decimals = minDecimals,
                                Quantity = 5,
                                //UsdBidAsk = new BidAsk {Ask = 0.04m, Bid = 0.06m},
                                UsdPriceAtCreation = 0.05m,
                                UsdWeightAtCreation = 0.25
                            },
                            new Component()
                            {
                                Symbol = "SYM2",
                                Decimals = maxDecimals,
                                Quantity = BigInteger.Multiply(5, BigInteger.Pow(10, decimalDiff)),
                                //UsdBidAsk = new BidAsk {Ask = 0.16m, Bid = 0.14m},
                                UsdPriceAtCreation = 0.15m,
                                UsdWeightAtCreation = 0.75
                            }
                        }
                    };
                    return true;
                });
            return indexProvider;
        }

        [Fact]
        public async Task CalculateKaikoNav_should_get_aggregated_prices_from_KaikoClient()
        {
            var nav = await _navCalculator.CalculateKaikoNav(KnownIndexes.L1CPU003, "USD")
                .ConfigureAwait(false);
            nav.Should().Be(1.075m);
        }

        [Fact]
        public async Task CalculateMessariNav_should_get_prices_from_MessariClient()
        {
            var nav = await _navCalculator.CalculateMessariNav(KnownIndexes.L1CPU003)
                .ConfigureAwait(false);
            nav.Should().Be(1m);
        }

        [Fact]
        public async Task CalculateCryptoCompareNav_should_get_prices_from_CryptoCompareClient()
        {
            var nav = await _navCalculator.CalculateCryptoCompareNav(KnownIndexes.L1CPU003)
                .ConfigureAwait(false);
            nav.Should().Be(1.25m);
        }

        [Fact]
        public async Task GetCryptoCompareIndexDetailsPriced_should_populate_details_with_current_prices()
        {
            var priced = await _navCalculator.GetCryptoCompareIndexDetailsPriced(KnownIndexes.L1CPU003)
                .ConfigureAwait(false);

            priced.BidAskNav.Ask.Should().Be(1.25m);
            
            var sym1 = priced.Components.Single(c => c.Symbol == "SYM1");
            sym1.UsdWeight.Should().Be(0.4);
            sym1.UsdValue.Should().Be(0.5m);
            sym1.UsdPriceAtCreation.Should().Be(0.05m);

            var sym2 = priced.Components.Single(c => c.Symbol == "SYM2");
            sym2.UsdWeight.Should().Be(0.6);
            sym2.UsdValue.Should().Be(0.75m);
            sym2.UsdPriceAtCreation.Should().Be(0.15m);
        }
    }

    public class MockedCryptoCompareHttpHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var price = request.RequestUri.AbsoluteUri.Contains("SYM1") ? 0.10m : 0.15m;
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent($"{{\"USD\":{price}}}");
            return Task.FromResult(response);
        }
    }
}