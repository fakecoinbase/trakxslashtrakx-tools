using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Sources.Kaiko;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;
using Trakx.Data.Market.Common.Sources.Messari.Client;

namespace Trakx.Data.Market.Common.Pricing
{
    public class NavCalculator
    {
        private readonly IKaikoClient _kaikoClient;
        private readonly IMessariClient _messariClient;
        private readonly IIndexDetailsProvider _indexDetailsProvider;
        private readonly ILogger<NavCalculator> _logger;

        public NavCalculator(IKaikoClient kaikoClient,
            IMessariClient messariClient,
            IIndexDetailsProvider indexDetailsProvider,
            ILogger<NavCalculator> logger)
        {
            _kaikoClient = kaikoClient;
            _messariClient = messariClient;
            _indexDetailsProvider = indexDetailsProvider;
            _logger = logger;
        }

        #region Kaiko
        public async Task<decimal> CalculateKaikoNav(KnownIndexes index, string quoteSymbol)
        {
            if (!_indexDetailsProvider.IndexDetails.TryGetValue(index, out var details))
            {
                _logger.LogWarning($"Failed to retrieve {index}");
                return 0;
            }
            var components = details.Components.Select(c => c.Symbol);

            var getPricesTasks = components.Select(c => CreateKaikoCoinQuery(c))
                .Select(async q =>
                {
                    var aggregatedPrice = await _kaikoClient.GetAggregatedPrices(q)
                        .ConfigureAwait(false);
                    return new { Price = aggregatedPrice, Symbol = q.BaseAsset };
                }).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var nav = details.Components.Aggregate(0m, (a, c) =>
            {
                var scaledQuantity = (decimal)Math.Pow(10, 18 - c.Decimals) * (decimal)c.Quantity;
                var prices = getPricesTasks.Select(t => t.Result)
                    .Single(r => r.Symbol.Equals(c.Symbol, StringComparison.InvariantCultureIgnoreCase)).Price;
                var summedVolume = prices.Sum(p => decimal.Parse(p.Volume));
                var averagedPrice = prices.Sum(p => decimal.Parse(p.Price) * decimal.Parse(p.Volume)) / summedVolume;
                var componentValue = scaledQuantity * averagedPrice;
                var indexValue = a + componentValue / (decimal)details.NaturalUnit;
                return indexValue;
            });

            return nav;
        }

        private AggregatedPriceRequest CreateKaikoCoinQuery(string coinSymbol, DateTime? dateTime = null)
        {
            var queryTime = dateTime.HasValue ? DateTime.MinValue : DateTime.UtcNow;
            var query = new AggregatedPriceRequest
            {
                DataVersion = "latest",
                BaseAsset = coinSymbol.ToLower(),
                Commodity = "trades",
                Exchanges = Constants.TrustedExchanges,
                Interval = "1d",
                PageSize = 1000,
                QuoteAsset = "usd",
                StartTime = queryTime.AddDays(-1),
                EndTime = queryTime,
                Sources = true
            };
            return query;
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
    }
}
