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
        private readonly IIndexDefinitionProvider _indexProvider;

        public NavCalculator(
            CryptoCompareClient cryptoCompareClient,
            ICoinGeckoClient coinGeckoClient,
            IIndexDefinitionProvider indexDetailsProvider, 
            ILogger<NavCalculator> logger)
        {
            _cryptoCompareClient = cryptoCompareClient;
            _coinGeckoClient = coinGeckoClient;
            _indexProvider = indexDetailsProvider;
            _logger = logger;
        }

        private readonly ILogger<NavCalculator> _logger;

        #region CryptoCompare

        public async Task<decimal> CalculateNav(string indexSymbol)
        {
            var priced = await GetIndexPriced(indexSymbol).ConfigureAwait(false);
            return priced.CurrentValuation.NetAssetValue;
        }

        public async Task<IndexPriced> GetIndexPriced(string indexSymbol)
        {
            var definition = await _indexProvider.GetDefinitionFromSymbol(indexSymbol)
                .ConfigureAwait(false);
            if (definition == IndexDefinition.Default) return IndexPriced.Default;

            var getPricesTasks = definition.ComponentDefinitions.Select(GetUsdPrice).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var componentsPriced = definition.ComponentDefinitions.Select(
                c =>
                {
                    var price = getPricesTasks.Single(t => t.Result.Key.Equals(c.Symbol)).Result.Value;
                    var valuation = new ComponentValuation(c, Usd, price);
                    return valuation;
                }).ToList();

            var indexPriced = new IndexPriced(definition, componentsPriced);

            return indexPriced;
        }

        private async Task<KeyValuePair<string, decimal>> GetUsdPrice(ComponentDefinition c)
        {
            try
            {
                var result = await GetCoinGeckoUsdPrice(c);
                return result;
            }
            catch
            {
                var fallback = await GetCryptoCompareUsdPrice(c);
                return fallback;
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
