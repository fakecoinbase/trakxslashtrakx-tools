using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinGecko.Clients;
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
        private readonly CoinsClient _coinsClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public CoinGeckoClient(HttpClient httpClient)
        {
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, c => TimeSpan.FromSeconds(c*c));
            _coinsClient = new global::CoinGecko.Clients.CoinsClient(httpClient);
        }

        public async Task<decimal> GetLatestUsdPrice(string symbol)
        {
            var coinList = await _retryPolicy
                .ExecuteAsync(() => _coinsClient.GetCoinList()).ConfigureAwait(false);

            var id = coinList.SingleOrDefault(c =>
                c.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))?.Id;

            if (id == null) return 0;

            var tickerDetails = await _retryPolicy.ExecuteAsync(
                () => _coinsClient.GetAllCoinDataWithId(id)).ConfigureAwait(false);
            var price = tickerDetails.MarketData.CurrentPrice["USD"];
            return (decimal?)price ?? 0m;
        }
    }
}