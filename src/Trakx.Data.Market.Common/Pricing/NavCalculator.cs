using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Logging;
using Trakx.Data.Models.Index;

namespace Trakx.Data.Market.Common.Pricing
{
    public class NavCalculator : INavCalculator
    {
        private const string Usd = "USD";
        private readonly CryptoCompareClient _cryptoCompareClient;
        private readonly IIndexDefinitionProvider _indexProvider;

        public NavCalculator(
            CryptoCompareClient cryptoCompareClient,
            IIndexDefinitionProvider indexDetailsProvider, 
            ILogger<NavCalculator> logger)
        {
            _cryptoCompareClient = cryptoCompareClient;
            _indexProvider = indexDetailsProvider;
            _logger = logger;
        }

        private readonly ILogger<NavCalculator> _logger;

        #region CryptoCompare

        public async Task<decimal> CalculateCryptoCompareNav(string indexSymbol)
        {
            var priced = await GetIndexPricedByCryptoCompare(indexSymbol).ConfigureAwait(false);
            return priced.CurrentValuation.NetAssetValue;
        }

        public async Task<IndexPriced> GetIndexPricedByCryptoCompare(string indexSymbol)
        {
            var definition = await _indexProvider.GetDefinitionFromSymbol(indexSymbol)
                .ConfigureAwait(false);
            if (definition == IndexDefinition.Default) return IndexPriced.Default;

            var getPricesTasks = definition.ComponentDefinitions.Select(async c =>
            {
                var price = await _cryptoCompareClient.Prices.SingleSymbolPriceAsync(c.Symbol, new[] { Usd })
                    .ConfigureAwait(false);
                return new { Price = price, c.Symbol };
            }).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var componentsPriced = definition.ComponentDefinitions.Select(
                c =>
                {
                    var price = getPricesTasks.Single(t => t.Result.Symbol == c.Symbol).Result.Price;
                    var priceUsd = price[Usd];
                    var valuation = new ComponentValuation(c, Usd, priceUsd);

                    return valuation;
                }).ToList();

            var indexPriced = new IndexPriced(definition, componentsPriced);

            return indexPriced;
        }
        #endregion
    }
}
