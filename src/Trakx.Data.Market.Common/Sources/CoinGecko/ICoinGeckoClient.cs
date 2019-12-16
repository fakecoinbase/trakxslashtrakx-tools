using System;
using System.Linq;
using System.Threading.Tasks;
using CoinGecko.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Retry;

namespace Trakx.Data.Market.Common.Sources.CoinGecko
{
    public interface ICoinGeckoClient
    {
        Task<decimal> GetLatestUsdPrice(string symbol);
    }

    public class CoinGeckoClient : ICoinGeckoClient
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICoinsClient _coinsClient;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ISimpleClient _simpleClient;

        public CoinGeckoClient(ClientFactory factory, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, c => TimeSpan.FromSeconds(c*c));
            _coinsClient = factory.CreateCoinsClient();
            _simpleClient = factory.CreateSimpleClient();
        }

        public async Task<decimal> GetLatestUsdPrice(string symbol)
        {
            var coinList = await _memoryCache.GetOrCreateAsync("CoinGecko.CoinList", async entry => 
                await _retryPolicy.ExecuteAsync(() => _coinsClient.GetCoinList()).ConfigureAwait(false));

            var id = coinList.FirstOrDefault(c =>
                c.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))?.Id;
            if (id == null) return 0;

            var tickerDetails = await _retryPolicy.ExecuteAsync(
                () => _simpleClient.GetSimplePrice(new []{id}, new []{"usd"})).ConfigureAwait(false);
            var price = tickerDetails[id]["usd"];
            return (decimal?)price ?? 0m;
        }
    }
}