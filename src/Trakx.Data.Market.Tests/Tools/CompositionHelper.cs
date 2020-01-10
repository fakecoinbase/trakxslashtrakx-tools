using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinGecko.Clients;
using Microsoft.Extensions.DependencyInjection;
using Polly.Retry;
using Trakx.Data.Market.Common.Sources.Messari.Client;
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

        public CompositionHelper(ITestOutputHelper output)
        {
            _output = output;
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMessariClient();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _messariClient = _serviceProvider.GetRequiredService<IMessariClient>();
        }

        [Fact]
        public async Task CheckTokensForErc20Implementation()
        {
            var assets = await _messariClient.GetAllAssets().ConfigureAwait(false);
            _output.WriteLine(assets.Count.ToString());
            var allCategories = assets.Select(a => a.Profile.Sector).Distinct();

            var omg = await _messariClient.GetProfileForSymbol("omg");


            _output.WriteLine(string.Join(", ", allCategories));
            ///var coinData = await _coinsClient.GetCoinList();
            //var coinIds = coinData.Where(d => _tokens.Contains(d.Symbol)).Select(d => d.Id);
        }
        

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClientHandler?.Dispose();
        }
    }
}