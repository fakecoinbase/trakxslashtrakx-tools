using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Retry;
using Trakx.Data.Common.Pricing;
using Trakx.Data.Common.Utils;

namespace Trakx.Data.Common.Sources.CoinGecko
{
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

        /// <inheritdoc />
        public async Task<decimal?> GetLatestPrice(string symbol, string quoteCurrency = Constants.DefaultQuoteCurrency)
        {
            var id = await GetCoinGeckoIdFromSymbol(symbol);
            if (id == default) return 0;

            var quoteCurrencyId = quoteCurrency.ToLower() == Constants.Usd 
                ? Constants.Usd 
                : await GetCoinGeckoIdFromSymbol(quoteCurrency);
            if (quoteCurrencyId == default) return 0;

            var tickerDetails = await _retryPolicy.ExecuteAsync(
                () => _simpleClient.GetSimplePrice(new []{id, quoteCurrencyId }, new []{ Constants.Usd }))
                .ConfigureAwait(false);
            var price = tickerDetails[id][Constants.Usd];
            var conversionToQuoteCurrency = tickerDetails[quoteCurrencyId][Constants.Usd];
            return (decimal?)(price / conversionToQuoteCurrency) ?? 0m;
        }

        private async Task<string?> GetCoinGeckoIdFromSymbol(string symbol)
        {
            var coinList = await GetCoinList();

            var id = coinList.FirstOrDefault(c =>
                c.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))?.Id;
            return id;
        }

        /// <inheritdoc />
        public async Task<decimal?> GetPriceAsOf(string symbol, DateTime asOf, string quoteCurrency = Constants.DefaultQuoteCurrency)
        {
            var id = await GetCoinGeckoIdFromSymbol(symbol);
            if (id == default) return 0;

            var quoteId = await GetCoinGeckoIdFromSymbol(quoteCurrency);
            var date = asOf.ToString("dd-MM-yyyy");
            
            var conversion = 1m;
            if (quoteId != default)
            {
                var quoteResponse = await _retryPolicy.ExecuteAsync(() => 
                    _coinsClient.GetHistoryByCoinId(id, date, false.ToString()));
                conversion = (decimal?)quoteResponse.MarketData.CurrentPrice[Constants.Usd] ?? 1m;
            }

            var historicalPrice = await _retryPolicy.ExecuteAsync(() => 
                _coinsClient.GetHistoryByCoinId(id, date, false.ToString()));

            return (decimal?)historicalPrice.MarketData.CurrentPrice[Constants.Usd] / conversion;
        }

        public bool TryRetrieveSymbol(string coinName, out string? symbol)
        {
            var coinList = GetCoinList().ConfigureAwait(false).GetAwaiter().GetResult();
            var symbolsByNames = coinList.ToDictionary(c => c.Name, c => c.Symbol);
            var bestMatch = coinName.FindBestLevenshteinMatch(symbolsByNames.Keys);
            symbol = bestMatch != null ? symbolsByNames[bestMatch] : null;

            return bestMatch != null;
        }

        public async Task<IReadOnlyList<CoinList>> GetCoinList()
        {
            var coinList = await _memoryCache.GetOrCreateAsync("CoinGecko.CoinList", async entry =>
                await _retryPolicy.ExecuteAsync(() => _coinsClient.GetCoinList()).ConfigureAwait(false));
            return coinList;
        }

        public IReadOnlyList<CoinList> CoinList  => _memoryCache.GetOrCreate("CoinGecko.CoinList", 
            entry => _retryPolicy.ExecuteAsync(() => _coinsClient.GetCoinList())
                .ConfigureAwait(false).GetAwaiter().GetResult());

    }
}