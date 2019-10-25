using System;
using System.Linq;
using Trakx.MarketApi.DataSources.Kaiko;
using Trakx.MarketApi.DataSources.Kaiko.Client;
using Trakx.MarketApi.DataSources.Kaiko.DTOs;
using Trakx.MarketApi.Indexes;

namespace Trakx.MarketApi.Pricing
{
    public class NavCalculator
    {
        private readonly RequestHelper _requestHelper;

        public NavCalculator(RequestHelper requestHelper)
        {
            _requestHelper = requestHelper;
        }

        public string CalculateNav(KnownIndexes index, string quoteSymbol)
        {
            if (!TrackerDetails.IndexDetails.TryGetValue(index, out var details))
                return $"failed to retrieve details for index {index}";
            var components = details.Components.Select(c => c.Symbol);

            components.Select(c => CreateCoinQuery(c)).AsParallel().Select(async q =>
            {
                var aggregatedPrice = await _requestHelper.GetAggregatedPrices(q)
                    .ConfigureAwait(false);
                return aggregatedPrice;
            });

            return "hello";
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
