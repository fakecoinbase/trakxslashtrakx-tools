using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Sources.Kaiko;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;

namespace Trakx.Data.Market.Common.Pricing
{
    public class NavCalculator
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IIndexDetailsProvider _indexDetailsProvider;

        public NavCalculator(IRequestHelper requestHelper, 
            IIndexDetailsProvider indexDetailsProvider)
        {
            _requestHelper = requestHelper;
            _indexDetailsProvider = indexDetailsProvider;
        }

        public async Task<string> CalculateKaikoNav(KnownIndexes index, string quoteSymbol)
        {
            if (!_indexDetailsProvider.IndexDetails.TryGetValue(index, out var details))
                return $"failed to retrieve details for index {index}";
            var components = details.Components.Select(c => c.Symbol);

            var getPricesTasks = components.Select(c => CreateKaikoCoinQuery(c))
                .Select(async q =>
                {
                    var aggregatedPrice = await _requestHelper.GetAggregatedPrices(q)
                        .ConfigureAwait(false);
                    return new {Price = aggregatedPrice, Symbol = q.BaseAsset};
                }).ToArray();

            await Task.WhenAll(getPricesTasks).ConfigureAwait(false);

            var nav = details.Components.Aggregate(0m, (a, c) =>
            {
                var scaledQuantity = (decimal)Math.Pow(10, 18 - c.Decimals) * (decimal) c.Quantity;
                var prices = getPricesTasks.Select(t => t.Result)
                    .Single(r => r.Symbol.Equals(c.Symbol, StringComparison.InvariantCultureIgnoreCase)).Price;
                var summedVolume = prices.Sum(p => decimal.Parse(p.Volume));
                var averagedPrice = prices.Sum(p => decimal.Parse(p.Price) * decimal.Parse(p.Volume)) / summedVolume;
                var componentValue = scaledQuantity * averagedPrice;
                var indexValue = a + componentValue / ((decimal)details.NaturalUnit);
                return indexValue;
            });

            return nav.ToString();
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

    }
}
