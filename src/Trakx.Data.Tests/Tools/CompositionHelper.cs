using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Common.Ethereum;
using Trakx.Data.Common.Sources.Coinbase;
using Trakx.Data.Common.Sources.CoinGecko;
using Trakx.Data.Common.Sources.Messari.Client;
using Trakx.Data.Common.Sources.Messari.DTOs;
using Trakx.Data.Common.Sources.Web3.Client;
using Xunit;
using Xunit.Abstractions;
using MarketData = Trakx.Data.Common.Sources.CoinGecko.MarketData;

namespace Trakx.Data.Tests.Tools
{
    public class CompositionHelper : IDisposable
    {
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private ITestOutputHelper _output;
        private ServiceProvider _serviceProvider;
        private IMessariClient _messariClient;
        private ICoinbaseClient _coinbaseClient;
        private ICoinGeckoClient _coinGeckoClient;
        private IWeb3Client _web3Client;
        private static readonly List<string> BadAssetNames = new List<string>() {"uniswap", "metacartel ventures", "tari" };

        private static readonly Dictionary<string, MarketData> MarketDataOverridesByCoinGeckoIdAndDate = new Dictionary<string, MarketData>
        {
            {"swissborg|20200101", new MarketData {MarketCap = 8_488_127m}},
            {"molecular-future|20200101", new MarketData {MarketCap = 75_298_069m}},
            {"rublix|20200101", new MarketData {MarketCap = 2_584_608m}},
            {"nectar-token|20200101", new MarketData {MarketCap = 6_833_195m}},
            {"mixin|20200101", new MarketData {MarketCap = 79_918_720m}},
        };

        public CompositionHelper(ITestOutputHelper output)
        {
            _output = output;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMessariClient();
            serviceCollection.AddCoinbaseClient();
            serviceCollection.AddCoinGeckoClient();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddEthereumInteraction();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            var conf = _serviceProvider.GetService<IConfiguration>();
            _messariClient = _serviceProvider.GetRequiredService<IMessariClient>();
            _coinbaseClient = _serviceProvider.GetRequiredService<ICoinbaseClient>();
            _coinGeckoClient = _serviceProvider.GetRequiredService<ICoinGeckoClient>();
            _web3Client = _serviceProvider.GetRequiredService<IWeb3Client>();
        }

        internal class ComponentLine
        {
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
            public bool IsErc20 => !string.IsNullOrWhiteSpace(ContractAddress)
                       && (TokenType?.Contains("ERC-20", StringComparison.InvariantCultureIgnoreCase) ?? false);
        }

        [Fact]
        public async Task GetDecimals()
        {
            var decimals =
                await _web3Client.GetDecimalsFromContractAddress("0xc80c5e40220172b36adee2c951f26f2a577810c5");
            decimals.Should().NotBe(0);
        }

        [Fact(Skip = "not a test")]
        public async Task GetCompositionCsv()
        {
            var allAssets = await _messariClient.GetAllAssets().ConfigureAwait(false);
            var assets = RemoveBadAssets(allAssets);

            var sectors = assets.Select(a => a?.Profile?.Sector ?? "Unknown")
                .Distinct().OrderBy(s => s).ToList();

            const string nullString = "null";
            _output.WriteLine($"\"{nameof(ComponentLine.Sector)}\"," +
                              $"\"{nameof(ComponentLine.TokenType)}\"," +
                              $"\"{nameof(ComponentLine.CoinbaseCustody)}\"," +
                              $"\"{nameof(ComponentLine.MessariSymbol)}\"," +
                              $"\"{nameof(ComponentLine.CoinGeckoSymbol)}\"," +
                              $"\"{nameof(ComponentLine.MessariName)}\"," +
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
                              $"\"{nameof(ComponentLine.Decimals)}\"");

            foreach (var sector in sectors.Where(s => s == "Scaling"))
            //foreach (var sector in sectors.Where(s => s == "Asset Management"))
            {
                var components = assets.Where(a =>
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

                    var coinbaseCustodied = _coinbaseClient.CustodiedCoins.Contains(componentSymbol,
                        StringComparer.InvariantCultureIgnoreCase);

                    _coinGeckoClient.RetrieveContractDetailsFromCoinSymbolName(componentSymbol, component.Name,
                        out var coinGeckoId, out var coinGeckoSymbol, out var contractAddress);
                    var marketData =
                        await _coinGeckoClient.GetMarketDataAsOfFromId(coinGeckoId, new DateTime(2020, 1, 1));

                    AddMarketDataOverrides(marketData);

                    var decimals = string.IsNullOrWhiteSpace(contractAddress)
                        ? 0
                        : await _web3Client.GetDecimalsFromContractAddress(contractAddress);

                    componentLines.Add(new ComponentLine
                    {
                        CoinbaseCustody = coinbaseCustodied,
                        CoinGeckoHistoricalUsdcPrice = marketData.Price ?? 0,
                        CoinGeckoId = coinGeckoId ?? nullString,
                        CoinGeckoSymbol = coinGeckoSymbol ?? nullString,
                        CoinGeckoHistoricalUsdcMarketCap = marketData.MarketCap ?? 0,
                        CoinGeckoHistoricalUsdcVolume = marketData.Volume,
                        ContractAddress = contractAddress,
                        CurrentMarketCapUsd = component.Metrics?.Marketcap?.CurrentMarketcapUsd ?? 0,
                        Decimals = decimals,
                        LiquidMarketCapUsd = component.Metrics?.Marketcap?.LiquidMarketcapUsd ?? 0,
                        MessariCurrentUsdPrice = component.Metrics?.MarketData?.PriceUsd ?? 0,
                        MessariName = component.Name ?? nullString,
                        MessariSymbol = component.Symbol?.ToLower()?.Trim() ?? nullString,
                        RealVolumeLast24Hours = component.Metrics?.MarketData?.RealVolumeLast24_Hours ?? 0,
                        Sector = sector,
                        TokenType = component.Profile?.TokenDetails?.Type?.Trim() ?? nullString,
                        VolumeLast24Hours = component.Metrics?.MarketData?.VolumeLast24_Hours ?? 0,
                        VolumeLast24HoursOverstatementMultiple =
                            component.Metrics?.MarketData?.VolumeLast24_HoursOverstatementMultiple ?? 0,
                        VolumeTurnoverLast24HoursPercent =
                            component.Metrics?.Marketcap?.VolumeTurnoverLast24_HoursPercent ?? 0
                    });
                }
                
                AssignComponentWeights(componentLines, 0.3m);

                foreach (var component in componentLines)
                {
                    _output.WriteLine($"\"{component.Sector}\"," +
                                      $"\"{component.TokenType}\"," +
                                      $"\"{component.CoinbaseCustody}\"," +
                                      $"\"{component.MessariSymbol}\"," +
                                      $"\"{component.CoinGeckoSymbol}\"," +
                                      $"\"{component.MessariName}\"," +
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
                                      $"\"{component.Decimals}\"");
                }
            }
        }

        public List<Asset> RemoveBadAssets(List<Asset> assetsToFilter)
        {
            var filtered = assetsToFilter.Where(a => !BadAssetNames.Contains(a?.Name?.Trim()?.ToLower())).ToList();
            return filtered;
        }

        public void AddMarketDataOverrides(MarketData marketData)
        {
            if (!MarketDataOverridesByCoinGeckoIdAndDate.TryGetValue($"{marketData.CoinId}|{marketData.AsOf:yyyyMMdd}",
                out var overrides))
                return;
            
            marketData.MarketCap = overrides.MarketCap;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClientHandler?.Dispose();
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
                    (c.IsErc20 && c.TargetWeight != 0 && c.TargetWeight < weightCap));

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
                    new decimal[] {10, 10, 20, 0, 50},
                    new decimal[] {10, 10, 10, 0, 10},
                    new decimal[] {10, 10, 20, 0, 6, 5, 10, 10},
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
