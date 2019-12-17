using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using CryptoCompare;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Market.Common.Sources.CoinGecko;
using Trakx.Data.Models.Index;
using Trakx.Data.Models.Initialisation;
using Xunit;

namespace Trakx.Data.Market.Tests.Unit.Common.Pricing
{
    public class NavCalculatorTests
    {
        private const string Usd = "Usd";
        private readonly NavCalculator _navCalculator;
        private readonly IndexDefinition[] _knownIndexes;
        private readonly ILogger<NavCalculator> _logger;
        private readonly ICoinGeckoClient _coinGeckoClient;

        public NavCalculatorTests()
        {
            _knownIndexes = DatabaseInitialiser.GetKnownIndexes();
            var indexProvider = PrepareIndexDefinitionProvider();

            var cryptoCompareClient = new CryptoCompareClient(
                new MockedCryptoCompareHttpHandler());

            _coinGeckoClient = Substitute.For<ICoinGeckoClient>();

            _logger = Substitute.For<ILogger<NavCalculator>>();

            _navCalculator = new NavCalculator(cryptoCompareClient, _coinGeckoClient, _logger);
        }

        private IIndexDefinitionProvider PrepareIndexDefinitionProvider()
        {
            var indexProvider = Substitute.For<IIndexDefinitionProvider>();
            indexProvider.GetDefinitionFromSymbol("IDX").Returns(_ =>
                {
                    var minDecimals = 8;
                    var maxDecimals = 18;
                    var firstQuantity = (ulong) 5;
                    var secondQuantity = (ulong) BigInteger.Multiply(5,
                        BigInteger.Pow(10, maxDecimals - minDecimals));

                    return new IndexDefinition("IDX",
                        "index",
                        "test index",
                        new List<ComponentDefinition>
                        {
                            new ComponentDefinition("0xabcd",
                                "Token 1",
                                "SYM1",
                                minDecimals,
                                firstQuantity,
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
                        naturalUnit: 18 - minDecimals,
                        DateTime.MinValue);
                });

            return indexProvider;
        }

        [Fact]
        public async Task CalculateCryptoCompareNav_should_get_prices_from_CryptoCompareClient()
        {
            var idx = await PrepareIndexDefinitionProvider().GetDefinitionFromSymbol("IDX");
            var nav = await _navCalculator.CalculateNav(idx)
                .ConfigureAwait(false);
            nav.Should().Be(1.25m);
        }

        [Fact]
        public async Task GetCryptoCompareIndexDetailsPriced_should_populate_details_with_current_prices()
        {
            var idx = await PrepareIndexDefinitionProvider().GetDefinitionFromSymbol("IDX");
            var priced = await _navCalculator.GetIndexPriced(idx)
                .ConfigureAwait(false);

            var price1i = priced.InitialValuation.ValuationsBySymbol["SYM1"];
            price1i.Price.Should().Be(0.05m);

            var price2i = priced.InitialValuation.ValuationsBySymbol["SYM2"];
            price2i.Price.Should().Be(0.15m);

            priced.InitialValuation.NetAssetValue.Should().Be(1m);

            var price1 = priced.CurrentValuation.ValuationsBySymbol["SYM1"];
            price1.Price.Should().Be(0.1m);

            var price2 = priced.CurrentValuation.ValuationsBySymbol["SYM2"];
            price2.Price.Should().Be(0.15m);

            priced.CurrentValuation.NetAssetValue.Should().Be(1.25m);
        }

        [Fact]
        public async Task CalculateCryptoCompareNav_should_double_when_prices_double()
        {
            var indexProvider = Substitute.For<IIndexDefinitionProvider>();
            indexProvider.GetDefinitionFromSymbol(Arg.Is<string>(s => _knownIndexes.Any(i => i.Symbol.Equals(s))))
                .Returns(ci => _knownIndexes.Single(s => s.Symbol.Equals(ci[0])));

            var strIndex = _knownIndexes.Single(i => i.Symbol.Equals("L1STR004"));
            var cryptoCompareClient =
                new CryptoCompareClient(
                new PriceDoublingCryptoCompareHttpHandler(strIndex.InitialValuation.Valuations.ToArray()));

             var navCalculator = new NavCalculator(cryptoCompareClient, _coinGeckoClient, _logger);

            var nav = await navCalculator.CalculateNav(strIndex)
                .ConfigureAwait(false);

            nav.Should().Be(strIndex.InitialValuation.NetAssetValue * 2);
        }
    }

    public class MockedCryptoCompareHttpHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var price = 0m;
            var parsed = QueryHelpers.ParseQuery(request.RequestUri.AbsoluteUri);
            var componentSymbol = parsed.First().Value;
            switch (componentSymbol)
            {
                case "SYM1": 
                    price = 0.10m;
                    break;
                case "SYM2":
                    price = 0.15m;
                    break;
                default:
                    price = 0m;
                    break;
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"{{\"USD\":{price}}}")
            };
            return Task.FromResult(response);
        }
    }

    public class PriceDoublingCryptoCompareHttpHandler : HttpClientHandler
    {
        private readonly ComponentValuation[] _initialValuations;

        public PriceDoublingCryptoCompareHttpHandler(ComponentValuation[] initialValuations)
        {
            _initialValuations = initialValuations;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var price = 0m;
            var parsed = QueryHelpers.ParseQuery(request.RequestUri.AbsoluteUri);
            var componentSymbol = parsed.First().Value;
            price = _initialValuations
                                .SingleOrDefault(i => i.Definition.Symbol.Equals(componentSymbol))?.Price * 2 ?? 0;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"{{\"USD\":{price}}}")
            };
            return Task.FromResult(response);
        }
    }
}