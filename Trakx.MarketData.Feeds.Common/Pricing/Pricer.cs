using System.Collections.Generic;
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
        public decimal CalculatePrice(string ticker, IList<decimal> componentPrices)
        {
            var tracker = new Tracker("ticker");
            return componentPrices.Average() * tracker.Leverage;
        }
    }
}