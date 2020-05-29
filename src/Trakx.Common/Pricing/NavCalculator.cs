using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Trakx.Common.Core;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Interfaces.Pricing;
using Trakx.Common.Sources.CoinGecko;
using Trakx.Common.Sources.Messari.Client;
using Trakx.Common.Utils;

namespace Trakx.Common.Pricing
{
    public class NavCalculator : INavCalculator
    {
        private readonly IMessariClient _messariClient;
        private readonly ICoinGeckoClient _coinGeckoClient;
        private readonly IDistributedCache _cache;
        private readonly ICryptoCompareClient _cryptoCompareClient;
        private readonly ILogger<NavCalculator> _logger;
        private readonly AsyncRetryPolicy _historicalPriceRetryPolicy;
        private readonly Dictionary<string, Func<IComponentDefinition, Task<KeyValuePair<string, SourcedPrice>>>> _priceFetchFunctionsBySource;

        public NavCalculator(IMessariClient messariClient,
            ICoinGeckoClient coinGeckoClient,
            IDistributedCache cache,
            ICryptoCompareClient cryptoCompareClient,
            ILogger<NavCalculator> logger)
        {
            _messariClient = messariClient;
            _coinGeckoClient = coinGeckoClient;
            _cache = cache;
            _cryptoCompareClient = cryptoCompareClient;
            _logger = logger;
            _historicalPriceRetryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(50 * i));

            _priceFetchFunctionsBySource = new Dictionary<string, Func<IComponentDefinition, Task<KeyValuePair<string, SourcedPrice>>>>
            {
                {"Cache", async c => await GetCachedPrice(c)},
                {"CryptoCompare", async c => await GetCryptoComparePrice(c)},
                {"Messari", async c => await GetMessariPrice(c)},
                {"CoinGecko", async c => await GetLatestCoinGeckoUsdPrice(c)},
            };
        }

        #region Implementation of INavCalculator

        /// <inheritdoc />
        public async Task<decimal> CalculateNav(IIndiceComposition indice,
            DateTime? asOf = default,
            string quoteCurrency = Constants.DefaultQuoteCurrency)
        {
            var valuation = await GetIndiceValuation(indice, asOf, quoteCurrency)
                .ConfigureAwait(false);
            return valuation.NetAssetValue;
        }

        /// <inheritdoc />
        public async Task<IIndiceValuation> GetIndiceValuation(IIndiceComposition composition,
            DateTime? asOf = default,
            string quoteCurrency = Constants.DefaultQuoteCurrency)
        {
            var utcNow = DateTime.UtcNow;
            asOf ??= utcNow;
            var utcTimeStamp = asOf.Value.ToUniversalTime();
            var getPricesTasks = composition.ComponentQuantities.Select(quantity =>
                asOf == utcNow ? GetLatestUsdPrice(quantity) : GetUsdPriceAsOf(quantity, asOf.Value)).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);
            var componentValuations = composition.ComponentQuantities.Select(
                c =>
                {
                    var sourcedPrice = getPricesTasks.Single(t => t.Result.Key.Equals(c.ComponentDefinition.Symbol)).Result.Value;
                    var valuation = new ComponentValuation(c, quoteCurrency, sourcedPrice.Price, sourcedPrice.Source, asOf.Value);
                    return (IComponentValuation)valuation;
                }).ToList();

            var indiceValuation = new IndiceValuation(composition, componentValuations, utcTimeStamp);

            return indiceValuation;
        }

        #endregion

        private async Task<KeyValuePair<string, SourcedPrice>> GetLatestUsdPrice(IComponentQuantity quantity)
        {
            foreach (var priceSource in _priceFetchFunctionsBySource)
            {
                try
                {
                    var result = await priceSource.Value(quantity.ComponentDefinition).ConfigureAwait(false);
                    if (result.Value.Price != default) return result;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Failed to retrieve price from {0}", priceSource.Key);
                }
            }
            
            throw new FailedToRetrievePriceException($"Failed to retrieve price for component {quantity.ComponentDefinition.Symbol}");
        }

        private async Task<KeyValuePair<string, SourcedPrice>> GetUsdPriceAsOf(IComponentQuantity quantity, DateTime asOf)
        {
            var result = await _historicalPriceRetryPolicy
                .ExecuteAndCaptureAsync(async () => await GetCoinGeckoUsdPriceAsOf(quantity.ComponentDefinition, asOf))
                .ConfigureAwait(false);

            if (result.Outcome == OutcomeType.Successful && result.Result.Value.Price != default)
                return result.Result;

            throw new FailedToRetrievePriceException($"Failed to retrieve price for component {quantity.ComponentDefinition.Symbol} as of {asOf}", result.FinalException);
        }

        private async Task<KeyValuePair<string, SourcedPrice>> GetMessariPrice(IComponentDefinition c)
        {
            var price = await _messariClient.GetLatestPrice(c.Symbol).ConfigureAwait(false);
            return price == default
                ? default
                : new KeyValuePair<string, SourcedPrice>(c.Symbol, new SourcedPrice(c.Symbol, "messari", price.Value));
        }

        private async ValueTask<KeyValuePair<string, SourcedPrice>> GetCachedPrice(IComponentDefinition c)
        {
            var priceBytes = await _cache.GetAsync(c.GetLatestPriceCacheKey("usdc")).ConfigureAwait(false);
            return priceBytes == default
                ? default
                : new KeyValuePair<string, SourcedPrice>(c.Symbol, new SourcedPrice(c.Symbol, "cryptoCompare", priceBytes.ToDecimal()));
        }

        private async ValueTask<KeyValuePair<string, SourcedPrice>> GetCryptoComparePrice(IComponentDefinition c)
        {
            var pricesByToCurrency = await _cryptoCompareClient.Prices.SingleSymbolPriceAsync(c.Symbol, new[] {"usdc"}, true)
                .ConfigureAwait(false);
            return pricesByToCurrency == default
                ? default
                : new KeyValuePair<string, SourcedPrice>(c.Symbol, new SourcedPrice(c.Symbol, "cryptoCompare", pricesByToCurrency["USDC"]));
        }

        private async ValueTask<KeyValuePair<string, SourcedPrice>> GetLatestCoinGeckoUsdPrice(IComponentDefinition c)
        {
            var price = await _coinGeckoClient.GetLatestPrice(c.CoinGeckoId).ConfigureAwait(false);
            return price == default
                ? default
                : new KeyValuePair<string, SourcedPrice>(c.Symbol, new SourcedPrice(c.Symbol, "coinGecko", price.Value));
        }

        private async ValueTask<KeyValuePair<string, SourcedPrice>> GetCoinGeckoUsdPriceAsOf(IComponentDefinition c, DateTime asOf)
        {
            var price = await _coinGeckoClient.GetPriceAsOfFromId(c.CoinGeckoId, asOf).ConfigureAwait(false);
            return price == default
                ? default
                : new KeyValuePair<string, SourcedPrice>(c.Symbol, new SourcedPrice(c.Symbol, "coinGecko", price.Value));
        }

        public class FailedToRetrievePriceException : Exception
        {
            public FailedToRetrievePriceException(string message) : base(message) { }
            public FailedToRetrievePriceException(string message, Exception innerException) : base(message, innerException) { }
        };
    }
}
