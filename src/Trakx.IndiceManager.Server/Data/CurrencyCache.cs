using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Trakx.Coinbase.Custody.Client.Interfaces;
using Trakx.Coinbase.Custody.Client.Models;

namespace Trakx.IndiceManager.Server.Data
{
    /// <inheritdoc />
    public class CurrencyCache : ICurrencyCache
    {
        private readonly ICoinbaseClient _coinbaseClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CurrencyCache> _logger;

        /// <inheritdoc />
        public CurrencyCache(ICoinbaseClient coinbaseClient,
            IMemoryCache memoryCache,
            ILogger<CurrencyCache> logger)
        {
            _coinbaseClient = coinbaseClient;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ushort?> GetDecimalsForCurrency(string symbol, 
            CancellationToken cancellationToken = default)
        {
            var currency = await GetCurrency(symbol, cancellationToken)
                .ConfigureAwait(false);
            return currency?.Decimals;
        }

        /// <inheritdoc />
        public async Task<Currency?> GetCurrency(string symbol, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_memoryCache.TryGetValue("coinbase_currency_" + symbol, out var cacheEntry))
                    return (Currency)cacheEntry;

                var currency = await _coinbaseClient.GetCurrencyAsync(symbol, cancellationToken)
                    .ConfigureAwait(false);
                _memoryCache.Set("coinbase_currency_" + symbol, currency, 
                    TimeSpan.FromDays(1));
                return currency;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get decimal amounts for currency from Coinbase Custody");
                return default;
            }
        }
    }
}