﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Ethereum;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Messari.Client;
using Trakx.Common.Sources.Messari.DTOs;
using Trakx.Common.Sources.Web3.Client;
using Trakx.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
using CgMarketData = Trakx.Common.Sources.CoinGecko.MarketData;

namespace Trakx.Drafts.Tools
{
    internal static class Constants
    {
        public static readonly string ApiEndpoint = @"https://data.messari.io/api/v1/";

        public static Dictionary<string, string> SectorSymbolBySector { get; } = 
            new Dictionary<string, string>
        {
            {"Asset Management", "amg"},
            {"Centralized Exchanges", "cex"},
            {"Decentralized Exchanges", "dex"},
            {"Lending", "len"},
            {"Scaling", "sca"},
        };
    }

    public class CompositionHelper
    {
        private const string NullString = "null";
        private readonly ITestOutputHelper _output;
        private readonly IMessariClient _messariClient;
        private readonly ICoinGeckoClient _coinGeckoClient;
        private readonly IWeb3Client _web3Client;
        private static readonly List<string> BadAssetNames = new List<string>() {"uniswap", "metacartel ventures", "tari", "synthetix network token" };

        private static readonly Dictionary<string, CgMarketData> MarketDataOverridesByCoinGeckoIdAndDate = 
            new Dictionary<string, CgMarketData>
        {
            {"swissborg|20200101", new CgMarketData {MarketCap = 8_488_127m}},
            {"molecular-future|20200101", new CgMarketData {MarketCap = 75_298_069m}},
            {"rublix|20200101", new CgMarketData {MarketCap = 2_584_608m}},
            {"nectar-token|20200101", new CgMarketData {MarketCap = 6_833_195m}},
            {"mixin|20200101", new CgMarketData {MarketCap = 79_918_720m}},

            {"bankera|20200301", new CgMarketData {MarketCap = 5_003_707m}},
            {"swissborg|20200301", new CgMarketData {MarketCap = 16_065_760m}},
            {"molecular-future|20200301", new CgMarketData {MarketCap = 50_620_349m}},
            {"rublix|20200301", new CgMarketData {MarketCap = 3_179_674m}},
            {"nectar-token|20200301", new CgMarketData {MarketCap = 6_926_546m}},
            {"mixin|20200301", new CgMarketData {MarketCap = 119_758_436m}},
        };

        private List<Asset> _assetsFromMessari;

        public CompositionHelper(ITestOutputHelper output)
        {
            _output = output;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMessariClient();
            serviceCollection.AddCoinGeckoClient();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddEthereumInteraction(Secrets.InfuraApiKey);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _messariClient = serviceProvider.GetRequiredService<IMessariClient>();
            _coinGeckoClient = serviceProvider.GetRequiredService<ICoinGeckoClient>();
            _web3Client = serviceProvider.GetRequiredService<IWeb3Client>();
        }

        internal class ComponentLine
        {
            #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
            public string Sector { get; set; }
            public string TokenType { get; set; }
            public bool CoinbaseCustody { get; set; }
            public string MessariSymbol { get; set; }
            public string MessariName { get; set; }
            public string CoinGeckoSymbol { get; set; }
            public string CoinGeckoId { get; set; }
            public double? CurrentMarketCapUsd { get; set; }
            public double? LiquidMarketCapUsd { get; set; }
            public decimal? VolumeLast24Hours { get; set; }
            public decimal? RealVolumeLast24Hours { get; set; }
            public double? VolumeTurnoverLast24HoursPercent { get; set; }
            public decimal? VolumeLast24HoursOverstatementMultiple { get; set; }
            public decimal? MessariCurrentUsdPrice { get; set; }
            public decimal? CoinGeckoHistoricalUsdcPrice { get; set; }
            public decimal CoinGeckoHistoricalUsdcMarketCap { get; set; }
            public decimal? CoinGeckoHistoricalUsdcVolume { get; set; }
            public decimal? TargetWeight { get; set; }
            public ushort? Decimals { get; set; }
            public string? ContractAddress { get; set; }
            public string? SectorSymbol { get; set; }
            public DateTime HistoricalAsOfDate { get; set; }
            public bool IsErc20 => !string.IsNullOrWhiteSpace(ContractAddress)
                       && (TokenType?.Contains("ERC-20", StringComparison.InvariantCultureIgnoreCase) ?? false);
            #pragma warning restore CS8618
        }

        [Fact]
        public async Task GetDecimals()
        {
            var decimals = await _web3Client.GetDecimalsFromContractAddress("0xc80c5e40220172b36adee2c951f26f2a577810c5");
            decimals.Should().NotBe(0);
        }

        [Fact(Skip = "not a test")]
        //[Fact]
        public async Task GetCompositionCsv()
        {
            var allAssets = await _messariClient.GetAllAssets().ConfigureAwait(false);
            var historicalAsOfDate = new DateTime(2020, 04, 01);

            _assetsFromMessari = RemoveBadAssets(allAssets);
            ReproduceThePastWithDodgyOverrides(_assetsFromMessari, historicalAsOfDate);

            WriteHeaderRow();

            await OutputComponentLinesForSectorAndDate(historicalAsOfDate/*, new []{"Asset Management"}*/);
        }

        private async Task OutputComponentLinesForSectorAndDate(DateTime historicalAsOfDate, IEnumerable<string> sectors = default)
        {
            sectors = _assetsFromMessari.Select(a => a.Profile.Sector).ToList().Distinct();
            foreach (var sector in Constants.SectorSymbolBySector.Keys.Intersect(sectors))
            {
                var components = _assetsFromMessari.Where(a =>
                    a.Profile?.Sector != null
                    && a.Profile.Sector.Equals(sector, StringComparison.InvariantCultureIgnoreCase));

                var componentLines = new List<ComponentLine>();

                foreach (var component in components.OrderBy(c => c.Symbol))
                {
                    var componentSymbol = component.Symbol;
                    if (string.IsNullOrWhiteSpace(componentSymbol)

                        && _coinGeckoClient.TryRetrieveSymbol(component.Name, out var symbol))
                    {
                        componentSymbol = symbol.ToUpper();
                    }

                    //var coinbaseCustodied = _coinbaseClient.CustodiedCoins.Contains(componentSymbol,
                    //    StringComparer.InvariantCultureIgnoreCase);

                    _coinGeckoClient.RetrieveContractDetailsFromCoinSymbolName(componentSymbol, component.Name,
                        out var coinGeckoId, out var coinGeckoSymbol, out var contractAddress);
                    var marketData =
                        await _coinGeckoClient.GetMarketDataAsOfFromId(coinGeckoId, historicalAsOfDate);

                    AddMarketDataOverrides(marketData);

                    var decimals = string.IsNullOrWhiteSpace(contractAddress)
                        ? 0
                        : await _web3Client.GetDecimalsFromContractAddress(contractAddress);

                    componentLines.Add(new ComponentLine
                    {
                        //CoinbaseCustody = coinbaseCustodied,
                        CoinGeckoHistoricalUsdcPrice = marketData.Price ?? 0,
                        CoinGeckoId = coinGeckoId ?? NullString,
                        CoinGeckoSymbol = coinGeckoSymbol ?? NullString,
                        CoinGeckoHistoricalUsdcMarketCap = marketData.MarketCap ?? 0,
                        CoinGeckoHistoricalUsdcVolume = marketData.Volume,
                        ContractAddress = contractAddress,
                        CurrentMarketCapUsd = component.Metrics?.Marketcap?.CurrentMarketcapUsd ?? 0,
                        Decimals = decimals,
                        LiquidMarketCapUsd = component.Metrics?.Marketcap?.LiquidMarketcapUsd ?? 0,
                        MessariCurrentUsdPrice = component.Metrics?.MarketData?.PriceUsd ?? 0,
                        MessariName = component.Name ?? NullString,
                        MessariSymbol = component.Symbol?.ToLower()?.Trim() ?? NullString,
                        RealVolumeLast24Hours = component.Metrics?.MarketData?.RealVolumeLast24_Hours ?? 0,
                        Sector = sector,
                        TokenType = component.Profile?.TokenDetails?.Type?.Trim() ?? NullString,
                        VolumeLast24Hours = component.Metrics?.MarketData?.VolumeLast24_Hours ?? 0,
                        VolumeLast24HoursOverstatementMultiple =
                            component.Metrics?.MarketData?.VolumeLast24_HoursOverstatementMultiple ?? 0,
                        VolumeTurnoverLast24HoursPercent =
                            component.Metrics?.Marketcap?.VolumeTurnoverLast24_HoursPercent ?? 0,
                        SectorSymbol = Constants.SectorSymbolBySector[sector],
                        HistoricalAsOfDate = historicalAsOfDate
                    });
                }

                AssignComponentWeights(componentLines, 0.3m);

                foreach (var component in componentLines.Where(c => c.TargetWeight != 0))
                {
                    WriteComponentRow(component);
                }
            }
        }

        private void WriteComponentRow(ComponentLine component)
        {
            _output.WriteLine($"\"{component.Sector}\"," +
                              $"\"{component.TokenType}\"," +
                              $"\"{component.CoinbaseCustody}\"," +
                              $"\"{component.MessariName}\"," +
                              $"\"{component.MessariSymbol}\"," +
                              $"\"{component.CoinGeckoSymbol}\"," +
                              $"\"{component.CoinGeckoId}\"," +
                              $"\"{component.CurrentMarketCapUsd}\"," +
                              $"\"{component.CoinGeckoHistoricalUsdcMarketCap}\"," +
                              $"\"{component.VolumeLast24Hours}\"," +
                              $"\"{component.RealVolumeLast24Hours}\"," +
                              $"\"{component.CoinGeckoHistoricalUsdcVolume}\"," +
                              $"\"{component.TargetWeight}\"," +
                              $"\"{component.MessariCurrentUsdPrice}\"," +
                              $"\"{component.CoinGeckoHistoricalUsdcPrice}\"," +
                              $"\"{component.ContractAddress}\"," +
                              $"\"{component.SectorSymbol}\"," +
                              $"\"{component.HistoricalAsOfDate:dd-MMM-yyyy}\"," +
                              $"\"{component.Decimals}\"");
        }

        private void WriteHeaderRow()
        {
            _output.WriteLine($"\"{nameof(ComponentLine.Sector)}\"," +
                              $"\"{nameof(ComponentLine.TokenType)}\"," +
                              $"\"{nameof(ComponentLine.CoinbaseCustody)}\"," +
                              $"\"{nameof(ComponentLine.MessariName)}\"," +
                              $"\"{nameof(ComponentLine.MessariSymbol)}\"," +
                              $"\"{nameof(ComponentLine.CoinGeckoSymbol)}\"," +
                              $"\"{nameof(ComponentLine.CoinGeckoId)}\"," +
                              $"\"{nameof(ComponentLine.CurrentMarketCapUsd)}\"," +
                              $"\"{nameof(ComponentLine.CoinGeckoHistoricalUsdcMarketCap)}\"," +
                              $"\"{nameof(ComponentLine.VolumeLast24Hours)}\"," +
                              $"\"{nameof(ComponentLine.RealVolumeLast24Hours)}\"," +
                              $"\"{nameof(ComponentLine.CoinGeckoHistoricalUsdcVolume)}\"," +
                              $"\"{nameof(ComponentLine.TargetWeight)}\"," +
                              $"\"{nameof(ComponentLine.MessariCurrentUsdPrice)}\"," +
                              $"\"{nameof(ComponentLine.CoinGeckoHistoricalUsdcPrice)}\"," +
                              $"\"{nameof(ComponentLine.ContractAddress)}\"," +
                              $"\"{nameof(ComponentLine.SectorSymbol)}\"," +
                              $"\"{nameof(ComponentLine.HistoricalAsOfDate)}\"," +
                              $"\"{nameof(ComponentLine.Decimals)}\"");
        }

        public List<Asset> RemoveBadAssets(List<Asset> assetsToFilter)
        {
            
            var filtered = assetsToFilter.Where(a => 
                !BadAssetNames.Contains(a?.Name?.Trim()?.ToLower())
            ).ToList();
            return filtered;
        }

        private static void ReproduceThePastWithDodgyOverrides(List<Asset> assets, DateTime historicalAsOfDate)
        {
            var firstJan = new DateTime(2020, 01, 01);
            if (historicalAsOfDate > firstJan) return;

            assets.RemoveAll(a => a.Name == "ROOBEE");
        }

        private void AddMarketDataOverrides(CgMarketData marketData)
        {
            if (!MarketDataOverridesByCoinGeckoIdAndDate.TryGetValue($"{marketData.CoinId}|{marketData.AsOf:yyyyMMdd}",
                out var overrides))
                return;
            
            marketData.MarketCap = overrides.MarketCap;
        }

        private static void AssignComponentWeights(List<ComponentLine> componentLines, decimal weightCap)
        {
            var erc20s = componentLines.Where(c => c.IsErc20).ToList();
            
            var nonNullMarketCaps = erc20s.Count(c => c.CoinGeckoHistoricalUsdcMarketCap != 0);

            var totalMarketCap = erc20s.Sum(c => c.CoinGeckoHistoricalUsdcMarketCap);

            if (nonNullMarketCaps == 0) weightCap = 0;

            else if (decimal.Round(weightCap, 10) < 1 / (decimal) nonNullMarketCaps)
            {
                weightCap = 1 / (decimal) nonNullMarketCaps;
            }
            
            var unassigned = 0m;
            do
            {
                var componentsAcceptingMoreWeight = componentLines.Count(c =>
                    !c.TargetWeight.HasValue ||
                    c.IsErc20 && c.TargetWeight != 0 && c.TargetWeight < weightCap);

                if (componentsAcceptingMoreWeight == 0) return;
                var weightToReassign = unassigned / componentsAcceptingMoreWeight;

                foreach (var component in componentLines)
                {
                    if (!component.TargetWeight.HasValue)
                    {
                        var rawWeight = component.IsErc20
                            ? component.CoinGeckoHistoricalUsdcMarketCap / totalMarketCap
                            : 0;
                        component.TargetWeight = Math.Min(rawWeight, 0.3m);
                        unassigned += Math.Max(0, rawWeight - 0.3m);
                    }
                    else if(component.TargetWeight != weightCap && component.TargetWeight != 0)
                    {
                        var oldWeight = component.TargetWeight.Value;
                        var newWeight = oldWeight + weightToReassign;
                        component.TargetWeight = Math.Min(weightCap, newWeight);
                        var reassigned = component.TargetWeight.Value - oldWeight;
                        unassigned -= reassigned;
                    }
                }
            } while (decimal.Round(unassigned, 10) > 0);
        }

        public class AssignComponentWeightsTestData : TheoryData
        {
            public AssignComponentWeightsTestData()
            {
                var marketCaps = new List<decimal[]>
                {
                    new [] {10m, 10m, 20m, 0m, 50m},
                    new [] {10m, 10m, 10m, 0m, 10m},
                    new [] {10m, 10m, 20m, 0m, 6m, 5m, 10m, 10m},
                };
                marketCaps.ForEach(caps =>
                {
                    var lines = caps.Select(c => new ComponentLine()
                    {
                        CoinGeckoHistoricalUsdcMarketCap = c,
                        TokenType = c != 6m ? "erc-20" : "native",
                        ContractAddress = "0xabc"
                    });
                    AddRow(lines.ToList());
                });
            }
        }

        [Theory]
        [ClassData(typeof(AssignComponentWeightsTestData))]
        internal void CheckWeightAssignment(List<ComponentLine> components)
        {
            AssignComponentWeights(components, 0.3m);
            components.ForEach(c => _output.WriteLine($"{c.CoinGeckoHistoricalUsdcMarketCap} => {c.TargetWeight}"));
            components.ForEach(c => c.TargetWeight.Should().BeLessOrEqualTo(0.3m));
            components.Sum(c => c.TargetWeight).Should().BeApproximately(1m, 0.001m);

            components.Where(c => !c.IsErc20).ToList().ForEach(c => c.TargetWeight.Should().Be(0m));
        }

        [Fact]
        internal void CheckWeightAssignment_overrides_when_too_low_cap()
        {
            var components = new decimal[] {10, 10, 10, 0, 10}
                .Select(c => new ComponentLine()
                {
                    CoinGeckoHistoricalUsdcMarketCap = c,
                    TokenType ="erc-20",
                    ContractAddress = "0xabc"

                }).ToList();
            AssignComponentWeights(components, 0.2m);
            components.Where(c => c.CoinGeckoHistoricalUsdcMarketCap != 0m).ToList()
                .ForEach(c => c.TargetWeight.Should().Be(0.25m));
        }


        [Fact]
        internal void CheckWeightAssignment_returns_zero_when_market_cap_sum_is_null()
        {
            var components = new decimal[] { 10, 10, 10, 0, 10 }
                .Select(c => new ComponentLine()
                {
                    //not erc20s, so they shouldn't get counted
                    CoinGeckoHistoricalUsdcMarketCap = c
                }).ToList();
            
            AssignComponentWeights(components, 0.3m);
            
            components.ForEach(c => c.TargetWeight.Should().Be(0));
        }
    }
}
