using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Sources.CoinGecko;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Common.Pricing
{
    public class NavCalculator : INavCalculator
    {
        private const string Usd = "USD";
        private readonly CryptoCompareClient _cryptoCompareClient;
        private readonly ICoinGeckoClient _coinGeckoClient;

        public NavCalculator(
            CryptoCompareClient cryptoCompareClient,
            ICoinGeckoClient coinGeckoClient,
            ILogger<NavCalculator> logger)
        {
            _cryptoCompareClient = cryptoCompareClient;
            _coinGeckoClient = coinGeckoClient;
            _logger = logger;
        }

        private readonly ILogger<NavCalculator> _logger;

        #region CryptoCompare

        public async Task<decimal> CalculateNav(IndexDefinition index)
        {
            var priced = await GetIndexPriced(index).ConfigureAwait(false);
            return priced.CurrentValuation.NetAssetValue;
        }

        public async Task<IndexPriced> GetIndexPriced(IndexDefinition index)
        {
            var getPricesTasks = index.ComponentDefinitions.Select(GetUsdPrice).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var componentsPriced = index.ComponentDefinitions.Select(
                c =>
                {
                    var price = getPricesTasks.Single(t => t.Result.Key.Equals(c.Symbol)).Result.Value;
                    var valuation = new ComponentValuation(c, Usd, price);
                    return valuation;
                }).ToList();

            var indexPriced = new IndexPriced(index, componentsPriced);

            return indexPriced;
        }

        private async Task<KeyValuePair<string, decimal>> GetUsdPrice(ComponentDefinition c)
        {
            try
            {
                var result = await GetCoinGeckoUsdPrice(c);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to retrieve price from CoinGecko");
            }

            try
            {
                var fallback = await GetCryptoCompareUsdPrice(c);
                return fallback;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to retrieve price from CryptoCompare");
            }

            throw new FailedToRetrievePriceException($"Failed to retrieve USD price for component {c.Symbol}");
        }

        private async Task<KeyValuePair<string, decimal>> GetCryptoCompareUsdPrice(ComponentDefinition c)
        {
            var price = await _cryptoCompareClient.Prices.SingleSymbolPriceAsync(c.Symbol, new[] {Usd})
                .ConfigureAwait(false);
            return new KeyValuePair<string, decimal>(c.Symbol, price[Usd]);
        }

        private async Task<KeyValuePair<string, decimal>> GetCoinGeckoUsdPrice(ComponentDefinition c)
        {
            var price = await _coinGeckoClient.GetLatestUsdPrice(c.Symbol)
                .ConfigureAwait(false);
            return new KeyValuePair<string, decimal>(c.Symbol, price);
        }

        private class FailedToRetrievePriceException : Exception
        {
            public FailedToRetrievePriceException(string message) : base(message) { }
        };

        #endregion
    }
}
