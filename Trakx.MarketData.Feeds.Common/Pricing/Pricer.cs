using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Trakx.MarketData.Feeds.Common.Models.Trakx;
using Trakx.MarketData.Feeds.Common.Trackers;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public class Pricer : IPricer
    {
        public Pricer()
        {
        }
        /// <inheritdoc />
        public IDictionary<string, decimal> CalculatePricesByCurrencies(
            string ticker,
            IList<IReadOnlyDictionary<string, decimal>> componentPriceByCurrency)
        {
            var tracker = new Tracker(ticker);
            var currencies = componentPriceByCurrency.SelectMany(p => p.Keys).Distinct();
            var result = currencies.ToDictionary(
                c => c,
                c => componentPriceByCurrency.Average(p => p[c]) * tracker.Leverage);

            return result;
        }

        /// <inheritdoc />
        public IDictionary<string, decimal> CalculatePricesByCurrencies(
            string ticker,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>> priceByCurrencyByTicker)
        {
            var tracker = new Tracker(ticker);
            var currencies = priceByCurrencyByTicker.Values.SelectMany(v => v.Keys).Distinct();
            var result = currencies.ToDictionary(
                c => c,
                c => priceByCurrencyByTicker.Average(p => p.Value[c]) * tracker.Leverage);

            return result;
        }
    }
}