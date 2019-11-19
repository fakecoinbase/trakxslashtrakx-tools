using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Pricing;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;
using Trakx.Data.Market.Tests.Data.Kaiko.AggregatedPrice;
using Xunit;

namespace Trakx.Data.Market.Tests.Unit.Common.Pricing
{
    public class NavCalculatorTests
    {
        private readonly NavCalculator _navCalculator;

        public NavCalculatorTests()
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
                            new Component() {
                                Symbol = "SYM1",
                                Decimals = minDecimals, 
                                Quantity = 5,
                                UsdBidAsk = new BidAsk { Ask = 0.04m, Bid = 0.06m },
                                UsdValueAtCreation = 0.05m,
                                UsdWeightAtCreation = 0.5m
                            },
                            new Component() {
                                Symbol = "SYM2",
                                Decimals = maxDecimals,
                                Quantity = BigInteger.Multiply(5, BigInteger.Pow(10, decimalDiff)),
                                UsdBidAsk = new BidAsk { Ask = 0.16m, Bid = 0.14m },
                                UsdValueAtCreation = 0.15m,
                                UsdWeightAtCreation = 0.5m
                            }
                        }
                    };
                    return true;
                });

            var aggregatedPriceReader = new AggregatedPriceReader();
            var requestHelper = Substitute.For<IRequestHelper>();
            requestHelper.GetAggregatedPrices(Arg.Any<AggregatedPriceRequest>())
                .Returns(async callInfo =>
                {
                    var symbol = ((AggregatedPriceRequest) callInfo[0]).BaseAsset;
                    var prices = await aggregatedPriceReader.GetAggregatePriceForSymbol(symbol)
                        .ConfigureAwait(false);
                    return prices;
                });

            var logger = Substitute.For<ILogger<NavCalculator>>();

            _navCalculator = new NavCalculator(requestHelper, indexProvider, logger);
        }

        [Fact]
        public async Task CalculateKaikoNav_should_get_aggregated_prices_from_Kaiko_RequestHelper()
        {
            var nav = await _navCalculator.CalculateKaikoNav(KnownIndexes.L1CPU003, "usdc")
                .ConfigureAwait(false);
            nav.Should().Be(1m);
        }
    }
}