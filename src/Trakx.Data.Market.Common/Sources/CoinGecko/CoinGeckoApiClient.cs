using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Trakx.Data.Market.Common.Sources.CoinGecko
{
    public class CoinGeckoApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public CoinGeckoApiClient()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(@"https://api.coingecko.com/api/v3/") };
        }

        public async Task<List<ExchangeDetails>> GetTopTrustedExchanges(int topCount = 10)
        {
            if (topCount > 100) topCount = 100;
            var path = "exchanges?" + $"per_page={topCount}";
            var response = await _httpClient.GetAsync(path).ConfigureAwait(false);
            await using var content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var result = await JsonSerializer.DeserializeAsync<List<ExchangeDetails>>(content).ConfigureAwait(false);
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _httpClient?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
