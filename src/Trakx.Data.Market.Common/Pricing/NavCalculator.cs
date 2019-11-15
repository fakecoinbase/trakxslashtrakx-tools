using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Trakx.Data.Market.Common.Indexes;
using Trakx.Data.Market.Common.Sources.Kaiko;
using Trakx.Data.Market.Common.Sources.Kaiko.Client;
using Trakx.Data.Market.Common.Sources.Kaiko.DTOs;

namespace Trakx.Data.Market.Common.Pricing
{
    public class NavCalculator
    {
        private readonly RequestHelper _requestHelper;

        public NavCalculator(RequestHelper requestHelper)
        {
            _requestHelper = requestHelper;
        }

        public async Task<string> CalculateKaikoNav(KnownIndexes index, string quoteSymbol)
        {
            if (!TrackerDetails.IndexDetails.TryGetValue(index, out var details))
                return $"failed to retrieve details for index {index}";
            var components = details.Components.Select(c => c.Symbol);

            var getPriceTasks = components.Select(c => CreateCoinQuery(c))
                .Select(async q =>
                {
                    var aggregatedPrice = await _requestHelper.GetAggregatedPrices(q)
                        .ConfigureAwait(false);
                    return new {Price = aggregatedPrice, Symbol = q.BaseAsset};
                }).ToArray();

            await Task.WhenAll(getPriceTasks).ConfigureAwait(false);

            var nav = details.Components.Aggregate(BigInteger.Zero, (a, c) =>
            {
                var scaledQuantity = BigInteger.Multiply(c.Quantity, BigInteger.Pow(10, 18 - c.Decimals));
                var prices = getPriceTasks.Select(t => t.Result).Single(r => r.Symbol == c.Symbol).Price;
                var summedVolume = prices.Sum(a => decimal.Parse(a.Volume));
                var averagedPrice = prices.Average(a => decimal.Parse(a.Price) * decimal.Parse(a.Volume)) / summedVolume;
                var componentValue = BigInteger.Multiply(scaledQuantity, new BigInteger(averagedPrice));
                var indexValue = BigInteger.Add(a, componentValue);
                return BigInteger.Divide(indexValue, details.NaturalUnit);
            });

            return nav.ToString();
        }

        private AggregatedPriceRequest CreateCoinQuery(string coinSymbol, DateTime? dateTime = null)
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
