using System;
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
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;
using Trakx.Data.Market.Common.Sources.Messari.Client;
using Trakx.Data.Market.Tests.Data.Kaiko;
using Trakx.Data.Market.Tests.Data.Messari;
using Trakx.Data.Models.Index;
using Xunit;

namespace Trakx.Data.Market.Tests.Unit.Common.Pricing
{
    public class NavCalculatorTests
    {
        private const string Usd = "Usd";
        private readonly NavCalculator _navCalculator;

        public NavCalculatorTests()
        {
            var indexProvider = PrepareIndexDefinitionProvider();

            var aggregatedPriceReader = new KaikoReader();
            var kaikoClient = Substitute.For<IKaikoClient>();
            kaikoClient.GetSpotExchangeRate(Arg.Any<SpotExchangeRateRequest>())
                .Returns(async callInfo =>
                {
                    var symbol = ((SpotExchangeRateRequest) callInfo[0]).BaseAsset;
                    var prices = await aggregatedPriceReader.GetSpotExchangeRateForSymbol(symbol, false)
                        .ConfigureAwait(false);
                    return prices;
                });
            kaikoClient.CreateSpotExchangeRateRequest(Arg.Any<string>(), "usd")
                .Returns(ci => new SpotExchangeRateRequest() {BaseAsset = (string) ci[0]});

            var messariReader = new MessariReader();
            var messariClient = Substitute.For<IMessariClient>();
            messariClient.GetMetricsForSymbol(Arg.Any<string>())
                .Returns(async callInfo =>
                {
                    var symbol = (string) callInfo[0];
                    var price = await messariReader.GetAssetMetrics(symbol)
                        .ConfigureAwait(false);
                    return price;
                });

            var cryptoCompareClient = new CryptoCompareClient(new MockedCryptoCompareHttpHandler());

            var logger = Substitute.For<ILogger<NavCalculator>>();

            _navCalculator = new NavCalculator(cryptoCompareClient, indexProvider, logger);
        }

        private IIndexDefinitionProvider PrepareIndexDefinitionProvider()
        {
            var indexProvider = Substitute.For<IIndexDefinitionProvider>();
            indexProvider.GetDefinitionFromSymbol("IDX")
                .Returns(_ =>
                {
                    var minDecimals = 8;
                    var maxDecimals = 18;
                    var firstQuantity = (ulong) BigInteger.Multiply(5,
                        BigInteger.Pow(10, minDecimals - 1));
                    var secondQuantity = (ulong) BigInteger.Multiply(5,
                        BigInteger.Pow(10, maxDecimals - 1));

                    return new IndexDefinition("IDX",
                        "index",
                        "test index",
                        new List<ComponentDefinition>
                        {
                            new ComponentDefinition("0xabcd",
                                "Token 1",
                                "SYM2",
                                maxDecimals,
                                secondQuantity,
                                0.05m,
                                Usd,
                                DateTime.UtcNow),
                            new ComponentDefinition("0xEFGH", 
                                "Token 2", 
                                "SYM2",
                                maxDecimals, 
                                secondQuantity, 
                                0.15m, 
                                Usd, 
                                DateTime.UtcNow)
                        },
                        "erc20address",
                        DateTime.MinValue);
                });

            return indexProvider;
        }

        [Fact]
        public async Task CalculateCryptoCompareNav_should_get_prices_from_CryptoCompareClient()
        {
            var nav = await _navCalculator.CalculateCryptoCompareNav("IDX")
                .ConfigureAwait(false);
            nav.Should().Be(0.125m);
        }

        [Fact]
        public async Task GetCryptoCompareIndexDetailsPriced_should_populate_details_with_current_prices()
        {
            var priced = await _navCalculator.GetIndexPricedByCryptoCompare("IDX")
                .ConfigureAwait(false);

            var price1i = priced.InitialValuation.ValuationsBySymbol["SYM1"];
            price1i.Price.Should().Be(0.1m);
            price1i.Value.Should().Be(0.05m);

            var price2i = priced.InitialValuation.ValuationsBySymbol["SYM2"];
            price2i.Price.Should().Be(0.15m);
            price2i.Value.Should().Be(0.075m);

            var price1 = priced.CurrentValuation.ValuationsBySymbol["SYM1"];
            price1.Price.Should().Be(0.1m);
            price1.Value.Should().Be(0.05m);

            var price2 = priced.CurrentValuation.ValuationsBySymbol["SYM2"];
            price2.Price.Should().Be(0.15m);
            price2.Value.Should().Be(0.075m);

            priced.CurrentValuation.NetAssetValue.Should().Be(0.125m);
        }
    }

    public class MockedCryptoCompareHttpHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var price = request.RequestUri.AbsoluteUri.Contains("SYM1") ? 0.10m : 0.15m;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"{{\"USD\":{price}}}")
            };
            return Task.FromResult(response);
        }
    }
}