using System;
using System.Collections.Generic;
using System.Text;

using CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public interface IPricer
    {
        IDictionary<string, decimal> CalculatePricesByCurrencies(
            string ticker,
            IList<IReadOnlyDictionary<string, decimal>> componentPriceByCurrency);

        IDictionary<string, decimal> CalculatePricesByCurrencies(
            string ticker,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>> priceByCurrencyByTicker);
    }
}
