using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CoinGecko.Clients;
using Microsoft.Extensions.DependencyInjection;
using Polly.Retry;
using Trakx.Data.Market.Common.Sources.Coinbase;
using Trakx.Data.Market.Common.Sources.CoinGecko;
using Trakx.Data.Market.Common.Sources.Messari.Client;
using Trakx.Data.Market.Common.Sources.Messari.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Data.Market.Tests.Tools
{
    public class CompositionHelper : IDisposable
    {
        private readonly CoinsClient _coinsClient;
        private readonly string[] _tokens;
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;
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
            
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _messariClient = _serviceProvider.GetRequiredService<IMessariClient>();
            _coinbaseClient = _serviceProvider.GetRequiredService<ICoinbaseClient>();
            _coinGeckoClient = _serviceProvider.GetRequiredService<ICoinGeckoClient>();
        }

        [Fact(Skip = "not a test")]
        public async Task CheckTokensForErc20Implementation()
        {
            var assets = await _messariClient.GetAllAssets().ConfigureAwait(false);

            _output.WriteLine($"\"{nameof(Asset.Symbol)}\", " +
                              $"\"{nameof(Asset.Name)}\", " +
                              $"\"{nameof(Asset.Metrics.Marketcap.CurrentMarketcapUsd)}\", " +
                              $"\"{nameof(Asset.Metrics.Marketcap.LiquidMarketcapUsd)}\", " +
                              $"\"{nameof(Asset.Metrics.MarketData.VolumeLast24_HoursOverstatementMultiple)}\", " +
                              $"\"{nameof(Asset.Profile.TokenDetails.Type)}\", " +
                              $"CoinbaseCustody");

            foreach (var sector in _messariClient.SelectedSectors)
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

                    _output.WriteLine($"\"{componentSymbol.Trim()}\", " +
                                      $"\"{component.Name.Trim()}\", " +
                                      $"\"{component.Metrics.Marketcap.CurrentMarketcapUsd}\", " +
                                      $"\"{component.Metrics.Marketcap.LiquidMarketcapUsd}\", " +
                                      $"\"{component.Metrics.MarketData.VolumeLast24_HoursOverstatementMultiple}\", " +
                                      $"\"{component.Profile.TokenDetails.Type.Trim()}\", " +
                                      $"\"{coinbaseCustodied}\"");
                }
                _output.WriteLine("");
            }
        }
        

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClientHandler?.Dispose();
        }
    }
}