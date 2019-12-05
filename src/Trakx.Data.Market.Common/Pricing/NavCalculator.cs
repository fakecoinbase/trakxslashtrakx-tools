using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Sources.Kaiko;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Messari.Client;

namespace Trakx.Data.Market.Common.Pricing
{
    public class NavCalculator : INavCalculator
    {
        private readonly IKaikoClient _kaikoClient;
        private readonly IMessariClient _messariClient;
        private readonly CryptoCompareClient _cryptoCompareClient;
        private readonly IIndexDetailsProvider _indexDetailsProvider;

        public NavCalculator(IKaikoClient kaikoClient,
            IMessariClient messariClient,
            CryptoCompareClient cryptoCompareClient,
            IIndexDetailsProvider indexDetailsProvider, 
            ILogger<NavCalculator> logger)
        {
            _kaikoClient = kaikoClient;
            _messariClient = messariClient;
            _cryptoCompareClient = cryptoCompareClient;
            _indexDetailsProvider = indexDetailsProvider;
            _logger = logger;
        }

        private readonly ILogger<NavCalculator> _logger;

        #region Kaiko
        public async Task<decimal> CalculateKaikoNav(KnownIndexes index, string quoteSymbol)
        {
            if (!_indexDetailsProvider.IndexDetails.TryGetValue(index, out var details))
            {
                _logger.LogWarning($"Failed to retrieve {index}");
                return 0;
            }
            var components = details.Components.Select(c => c.Symbol);

            var getPricesTasks = components.Select(c => _kaikoClient.CreateSpotExchangeRateRequest(c, "usd"))
                .Select(async q =>
                {
                    var response = await _kaikoClient.GetSpotExchangeRate(q)
                        .ConfigureAwait(false);
                    if (response == null || response.Result != Constants.SuccessResponse || !response.Data.Any())
                    {
                        _logger.LogWarning("Failed to retrieve data for {q}", q.BaseAsset);
                    }
                    return response;
                }).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var nav = details.Components.Aggregate(0m, (a, c) =>
            {
                var scaledQuantity = (decimal)Math.Pow(10, 18 - c.Decimals) * (decimal)c.Quantity;
                var prices = getPricesTasks
                    .Select(t => t.Result)
                    .Single(r => r.Query.BaseAsset.Equals(c.Symbol, StringComparison.InvariantCultureIgnoreCase))
                    .Data
                    .OrderByDescending(d => d.Timestamp);
                var averagedPrice = decimal.Parse(prices.First().Price);
                var componentValue = scaledQuantity * averagedPrice;
                var indexValue = a + componentValue / (decimal)details.NaturalUnit;
                return indexValue;
            });

            return nav;
        }
        #endregion

        #region Messari
        public async Task<decimal> CalculateMessariNav(KnownIndexes index)
        {
            if (!_indexDetailsProvider.IndexDetails.TryGetValue(index, out var details))
            {
                _logger.LogWarning($"Failed to retrieve {index}");
                return 0;
            }

            var getPricesTasks = details.Components.Select(async c =>
                {
                    var metrics = await _messariClient.GetMetricsForSymbol(c.Symbol)
                        .ConfigureAwait(false);
                    return metrics;
                }).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var nav = details.Components.Aggregate(0m, (a, c) =>
            {
                var scaledQuantity = (decimal)Math.Pow(10, 18 - c.Decimals) * (decimal)c.Quantity;
                var metrics = getPricesTasks.Select(t => t.Result)
                    .Single(r => r.Symbol.Equals(c.Symbol, StringComparison.InvariantCultureIgnoreCase));
                var componentValue = scaledQuantity * metrics.MarketData.PriceUsd;
                var indexValue = a + componentValue / (decimal)details.NaturalUnit;
                return indexValue;
            });

            return nav;
        }
        #endregion

        #region CryptoCompare

        public async Task<decimal> CalculateCryptoCompareNav(KnownIndexes index)
        {
            if (!_indexDetailsProvider.IndexDetails.TryGetValue(index, out var details))
            {
                _logger.LogWarning($"Failed to retrieve {index}");
                return 0;
            }

            var getPricesTasks = details.Components.Select(async c =>
            {
                var price = await _cryptoCompareClient.Prices.SingleSymbolPriceAsync(c.Symbol, new [] {"USD"})
                    .ConfigureAwait(false);
                return new {Price = price, c.Symbol};
            }).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var nav = details.Components.Aggregate(0m, (a, c) =>
            {
                var scaledQuantity = (decimal)Math.Pow(10, 18 - c.Decimals) * (decimal)c.Quantity;
                var price = getPricesTasks.Single(t => t.Result.Symbol == c.Symbol).Result.Price;
                var componentValue = scaledQuantity * price["USD"];
                var indexValue = a + componentValue / (decimal)details.NaturalUnit;
                return indexValue;
            });

            return nav;
        }
        #endregion
    }
}
