using System;
using System.Collections.Generic;

using CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public interface IPricer
    {
        /// <inheritdoc />
        IDictionary<string, decimal> CalculatePricesByCurrencies(
            string ticker,
            IList<IReadOnlyDictionary<string, decimal>> componentPriceByCurrency);

        /// <inheritdoc />
        IDictionary<string, decimal> CalculatePricesByCurrencies(
            string ticker,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>> priceByCurrencyByTicker);

        decimal? LeveragedAverage(int leverage, IList<CoinFullAggregatedData> componentData, Func<CoinFullAggregatedData, decimal?> selector);

        double? LeveragedAverage(int leverage, IList<CoinFullAggregatedData> componentData, Func<CoinFullAggregatedData, double?> selector);

        decimal CalculateVolumeFromUnderlyingVolumeTo(List<decimal> componentVolumes);
    }
}
