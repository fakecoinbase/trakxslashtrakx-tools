using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Trakx.Common.Core;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces;
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
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<NavCalculator> _logger;
        private readonly Dictionary<string, 
            Func<IComponentDefinition, Task<SourcedPrice>>> _livePriceFetchFunctionsBySource;
        private readonly Dictionary<string, Func<DateTime, 
            Func<IComponentDefinition, Task<SourcedPrice>>>> _historicalPriceFetchFunctionsBySource;

        public NavCalculator(IMessariClient messariClient,
            ICoinGeckoClient coinGeckoClient,
            IDistributedCache cache,
            ICryptoCompareClient cryptoCompareClient,
            IDateTimeProvider dateTimeProvider,
            ILogger<NavCalculator> logger)
        {
            _messariClient = messariClient;
            _coinGeckoClient = coinGeckoClient;
            _cache = cache;
            _cryptoCompareClient = cryptoCompareClient;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;

            _livePriceFetchFunctionsBySource = new Dictionary<string, Func<IComponentDefinition, Task<SourcedPrice>>>
            {
                {"Cache", async c => await GetCachedPrice(c)},
                {"Messari", async c => await GetMessariPrice(c)},
                {"CoinGecko", async c => await GetLatestCoinGeckoUsdPrice(c)},
            };

            _historicalPriceFetchFunctionsBySource = new Dictionary<string, Func<DateTime, Func<IComponentDefinition, Task<SourcedPrice>>>>
            {
                {"CryptoCompare", d => async c => await GetCryptoCompareUsdPriceAsOf(c, d)},
                {"CoinGecko", d => async c => await GetCoinGeckoUsdPriceAsOf(c, d)},
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
            if (asOf > _dateTimeProvider.UtcNow) throw new ArgumentOutOfRangeException(nameof(asOf), asOf, "Cannot get a valuation for a future date.");

            var getPricesTasks = composition.ComponentQuantities.Select(quantity =>
                asOf == default
                    ? GetLatestUsdcPrice(quantity.ComponentDefinition)
                    : GetHistoricalUsdcPrice(quantity.ComponentDefinition, asOf!.Value)).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var valuationTime = asOf?.ToUniversalTime() ?? _dateTimeProvider.UtcNow;
            var componentValuations = composition.ComponentQuantities.Select(
                c =>
                {
                    var sourcedPrice = getPricesTasks.Single(t => t.Result.Symbol.Equals(c.ComponentDefinition.Symbol)).Result;
                    var valuation = new ComponentValuation(c, quoteCurrency, sourcedPrice.Price, sourcedPrice.Source, valuationTime);
                    return (IComponentValuation)valuation;
                }).ToList();

            var indiceValuation = new IndiceValuation(composition, componentValuations, valuationTime);

            return indiceValuation;
        }

        #endregion

        private async Task<SourcedPrice> GetLatestUsdcPrice(IComponentDefinition component)
        {
            return await GetUsdPriceWithFallbacks(component, _livePriceFetchFunctionsBySource).ConfigureAwait(false);
        }

        private async Task<SourcedPrice> GetHistoricalUsdcPrice(IComponentDefinition component, DateTime asOf)
        {
            var fetchFunctions = _historicalPriceFetchFunctionsBySource
                .ToDictionary(p => p.Key, p => p.Value(asOf));
            return await GetUsdPriceWithFallbacks(component, fetchFunctions, asOf).ConfigureAwait(false);
        }

        private async Task<SourcedPrice> GetUsdPriceWithFallbacks(IComponentDefinition component,
            Dictionary<string, Func<IComponentDefinition, Task<SourcedPrice>>> fetchFunctions,
            DateTime? asOf = default)
        {
            foreach (var priceSource in fetchFunctions)
            {
                try
                {
                    var result = await priceSource.Value(component).ConfigureAwait(false);
                    if (result.Price != default) return result;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Failed to retrieve price from {0}", priceSource.Key);
                }
            }

            throw new FailedToRetrievePriceException(
                $"Failed to retrieve price for component {component.Symbol} as of {asOf?.ToIso8601() ?? "now"}");
        }

        private async Task<SourcedPrice> GetMessariPrice(IComponentDefinition c)
        {
            var price = await _messariClient.GetLatestPrice(c.Symbol.ToNativeSymbol()).ConfigureAwait(false);
            return price == default
                ? default
                : new SourcedPrice(c.Symbol, "messari", price.Value);
        }

        private async ValueTask<SourcedPrice> GetCachedPrice(IComponentDefinition c)
        {
            var priceBytes = await _cache.GetAsync(c.GetLatestPriceCacheKey("usdc")).ConfigureAwait(false);
            return priceBytes == default
                ? default
                : new SourcedPrice(c.Symbol, "cryptoCompare", priceBytes.ToDecimal());
        }

        private async ValueTask<SourcedPrice> GetLatestCoinGeckoUsdPrice(IComponentDefinition c)
        {
            var price = await _coinGeckoClient.GetLatestPrice(c.CoinGeckoId).ConfigureAwait(false);
            return price == default
                ? default
                : new SourcedPrice(c.Symbol, "coinGecko", price.Value);
        }

        private async ValueTask<SourcedPrice> GetCoinGeckoUsdPriceAsOf(IComponentDefinition c, DateTime asOf)
        {
            var price = await _coinGeckoClient.GetPriceAsOfFromId(c.CoinGeckoId, asOf).ConfigureAwait(false);
            return price == default
                ? default
                : new SourcedPrice(c.Symbol, "coinGecko", price.Value);
        }

        /// <summary>
        /// With our current CryptoCompare subscription, we only have historical minute data for 7 days,
        /// and hourly for 3 years, the rest is daily history. This function tries to get the most accurate
        /// price given those restrictions.
        /// </summary>
        /// <param name="component">Definition of the component for which we want the price.</param>
        /// <param name="asOf">Date as of which we want the price.</param>
        /// <returns></returns>
        private async ValueTask<SourcedPrice> GetCryptoCompareUsdPriceAsOf(IComponentDefinition component, DateTime asOf)
        {
            var now = _dateTimeProvider.UtcNow;
            Func<Task<HistoryResponse>> fetchPrice;

            if (asOf > now.AddDays(-7))
                fetchPrice = () => _cryptoCompareClient.History.MinutelyAsync(component.Symbol, "usdc", 1, toDate: asOf, tryConversion: true);
            else if (asOf > now.AddYears(-3))
                fetchPrice = () => _cryptoCompareClient.History.HourlyAsync(component.Symbol, "usdc", 1, toDate: asOf, tryConversion: true);
            else
                fetchPrice = () => _cryptoCompareClient.History.DailyAsync(component.Symbol, "usdc", 1, toDate: asOf, tryConversion: true);
           
            var price = await fetchPrice().ConfigureAwait(false);
            return price == default
                ? default
                : new SourcedPrice(component.Symbol, "cryptoCompare", price.Data.Last().Close);
        }

        public class FailedToRetrievePriceException : Exception
        {
            public FailedToRetrievePriceException(string message) : base(message) { }
        };
    }
}
