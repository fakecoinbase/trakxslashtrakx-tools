using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Data.Common.Core;
using Trakx.Data.Common.Interfaces.Index;
using Trakx.Data.Common.Interfaces.Pricing;
using Trakx.Data.Common.Sources.CoinGecko;
using Trakx.Data.Common.Sources.Messari.Client;

namespace Trakx.Data.Common.Pricing
{
    public class NavCalculator : INavCalculator
    {
        private readonly IMessariClient _messariClient;
        private readonly ICoinGeckoClient _coinGeckoClient;

        public NavCalculator(IMessariClient messariClient,
            ICoinGeckoClient coinGeckoClient,
            ILogger<NavCalculator> logger)
        {
            _messariClient = messariClient;
            _coinGeckoClient = coinGeckoClient;
            _logger = logger;
        }

        private readonly ILogger<NavCalculator> _logger;

      
        private async Task<KeyValuePair<string, decimal?>> GetUsdPrice(IComponentQuantity c)
        {
            try
            {
                var result = await GetMessariPrice(c.ComponentDefinition);
                if (result.Value != default) return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to retrieve price from Messari");
            }

            try
            {
                var result = await GetCoinGeckoUsdPrice(c.ComponentDefinition);
                if (result.Value != default) return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to retrieve price from CoinGecko");
            }

            throw new FailedToRetrievePriceException($"Failed to retrieve price for component {c.ComponentDefinition.Symbol}");
        }

        private async Task<KeyValuePair<string, decimal?>> GetMessariPrice(IComponentDefinition c)
        {
            var price = await _messariClient.GetLatestPrice(c.Symbol).ConfigureAwait(false);
            return new KeyValuePair<string, decimal?>(c.Symbol, price);
        }

        private async Task<KeyValuePair<string, decimal?>> GetCoinGeckoUsdPrice(IComponentDefinition c)
        {
            var price = await _coinGeckoClient.GetLatestPrice(c.Symbol)
                .ConfigureAwait(false);
            return new KeyValuePair<string, decimal?>(c.Symbol, price);
        }

        public class FailedToRetrievePriceException : Exception
        {
            public FailedToRetrievePriceException(string message) : base(message) { }
        };

        #region Implementation of INavCalculator

        /// <inheritdoc />
        public async Task<decimal> CalculateNav(IIndexComposition index,
            DateTime? asOf = default, 
            string quoteCurrency = Constants.DefaultQuoteCurrency)
        {
            var valuation = await GetIndexValuation(index, asOf, quoteCurrency)
                .ConfigureAwait(false);
            return valuation.NetAssetValue;
        }

        /// <inheritdoc />
        public async Task<IIndexValuation> GetIndexValuation(IIndexComposition composition,
            DateTime? asOf = default, 
            string quoteCurrency = Constants.DefaultQuoteCurrency)
        {
            var utcTimeStamp = asOf?.ToUniversalTime() ?? DateTime.UtcNow;
            var getPricesTasks = composition.ComponentQuantities.Select(GetUsdPrice).ToArray();
            
            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);
            var componentValuations = composition.ComponentQuantities.Select(
                c =>
                {
                    var price = getPricesTasks.Single(t => t.Result.Key.Equals(c.ComponentDefinition.Symbol)).Result.Value;
                    var valuation = new ComponentValuation(c, quoteCurrency, price ?? 0, DateTime.UtcNow);
                    return (IComponentValuation) valuation;
                }).ToList();

            var indexValuation = new IndexValuation(composition, componentValuations, utcTimeStamp);

            return indexValuation;
        }

        #endregion
    }
}
