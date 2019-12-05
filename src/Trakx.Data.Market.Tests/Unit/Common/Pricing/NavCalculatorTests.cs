using System.Collections.Generic;
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
                                UsdBidAsk = new BidAsk {Ask = 0.04m, Bid = 0.06m},
                                UsdValueAtCreation = 0.05m,
                                UsdWeightAtCreation = 0.5m
                            },
                            new Component()
                            {
                                Symbol = "SYM2",
                                Decimals = maxDecimals,
                                Quantity = BigInteger.Multiply(5, BigInteger.Pow(10, decimalDiff)),
                                UsdBidAsk = new BidAsk {Ask = 0.16m, Bid = 0.14m},
                                UsdValueAtCreation = 0.15m,
                                UsdWeightAtCreation = 0.5m
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
            var nav = await _navCalculator.CalculateKaikoNav(KnownIndexes.L1CPU003, "usdc")
                .ConfigureAwait(false);
            nav.Should().Be(1m);
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
            nav.Should().Be(1m);
        }
    }

    public class MockedCryptoCompareHttpHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var price = request.RequestUri.AbsoluteUri.Contains("SYM1") ? 0.05m : 0.15m;
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent($"{{\"USD\":{price}}}");
            return Task.FromResult(response);
        }
    }
}