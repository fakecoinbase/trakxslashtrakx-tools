using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Data.Common.Sources.Coinbase;
using Trakx.Data.Common.Sources.CoinGecko;
using Trakx.Data.Common.Sources.Messari.Client;
using Trakx.Data.Common.Sources.Messari.DTOs;
using Xunit;
using Xunit.Abstractions;

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

        public CompositionHelper(ITestOutputHelper output)
        {
            _output = output;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMessariClient();
            serviceCollection.AddCoinbaseClient();
            serviceCollection.AddCoinGeckoClient();
            serviceCollection.AddMemoryCache();
            
            _serviceProvider = serviceCollection.BuildServiceProvider();
            var conf = _serviceProvider.GetService<IConfiguration>();
            _messariClient = _serviceProvider.GetRequiredService<IMessariClient>();
            _coinbaseClient = _serviceProvider.GetRequiredService<ICoinbaseClient>();
            _coinGeckoClient = _serviceProvider.GetRequiredService<ICoinGeckoClient>();
        }

        [Fact()]
        public async Task CheckTokensForErc20Implementation()
        {
            var assets = await _messariClient.GetAllAssets().ConfigureAwait(false);
            var sectors = assets.Select(a => a?.Profile?.Sector ?? "Unknown")
                .Distinct().OrderBy(s => s).ToList();

            _output.WriteLine($"\"{nameof(Asset.Symbol)}\"," +
                              $"\"{nameof(Asset.Name)}\"," +
                              $"\"{nameof(Asset.Metrics.Marketcap.CurrentMarketcapUsd)}\"," +
                              $"\"{nameof(Asset.Metrics.Marketcap.LiquidMarketcapUsd)}\"," +
                              $"\"{nameof(Asset.Metrics.MarketData.VolumeLast24_Hours)}\"," +
                              $"\"{nameof(Asset.Metrics.MarketData.RealVolumeLast24_Hours)}\"," +
                              $"\"{nameof(Asset.Metrics.Marketcap.VolumeTurnoverLast24_HoursPercent)}\"," +
                              $"\"{nameof(Asset.Metrics.MarketData.VolumeLast24_HoursOverstatementMultiple)}\"," +
                              $"\"{nameof(Asset.Metrics.MarketData.PriceUsd)}\"," +
                              $"\"{nameof(Asset.Profile.TokenDetails.Type)}\"," +
                              $"\"CoinGeckoSymbol\"," +
                              $"\"CoinGeckoId\"," +
                              $"\"PriceOn20200101\"," +
                              $"\"Link\"," +
                              $"CoinbaseCustody");

            foreach (var sector in sectors.Where(s => s != "Unknown"))
            {
                var components = assets.Where(a =>
                    a.Profile?.Sector != null
                    && a.Profile.Sector.Equals(sector, StringComparison.InvariantCultureIgnoreCase));
                _output.WriteLine($"\"{sector}\"");
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

                    _coinGeckoClient.RetrieveContractDetailsFromCoinSymbolName(componentSymbol, component.Name, out var coinGeckoId,out var coinGeckoSymbol, out var link);
                    var priceAsOfFirstJan = await _coinGeckoClient.GetPriceAsOf(componentSymbol, new DateTime(2020, 1, 1));

                    const string nullString = "null";

                    _output.WriteLine($"\"{componentSymbol?.Trim() ?? nullString}\"," +
                                      $"\"{component?.Name?.Trim() ?? nullString}\"," +
                                      $"\"{component?.Metrics?.Marketcap?.CurrentMarketcapUsd ?? 0}\"," +
                                      $"\"{component?.Metrics?.Marketcap?.LiquidMarketcapUsd ?? 0}\"," +
                                      $"\"{component?.Metrics?.MarketData.VolumeLast24_Hours ?? 0}\"," +
                                      $"\"{component?.Metrics?.MarketData.RealVolumeLast24_Hours ?? 0}\"," +
                                      $"\"{component?.Metrics?.Marketcap?.VolumeTurnoverLast24_HoursPercent ?? 0}\"," +
                                      $"\"{component?.Metrics?.MarketData?.VolumeLast24_HoursOverstatementMultiple ?? 0}\"," +
                                      $"\"{component?.Metrics?.MarketData?.PriceUsd ?? 0}\"," +
                                      $"\"{component?.Profile?.TokenDetails?.Type?.Trim() ?? nullString}\"," +
                                      $"\"{coinGeckoSymbol ?? nullString}\"," +
                                      $"\"{coinGeckoId ?? nullString}\"," +
                                      $"\"{priceAsOfFirstJan ?? 0}\"," +
                                      $"\"{link ?? nullString}\"," +
                                      $"\"{coinbaseCustodied}\"");
                }
                _output.WriteLine("");
            }
            _output.WriteLine("Sectors");
            _output.WriteLine(string.Join(Environment.NewLine, sectors));
        }
        

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClientHandler?.Dispose();
        }
    }
}