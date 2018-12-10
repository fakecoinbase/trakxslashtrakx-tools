using System;
using System.Collections.Generic;
using System.Text;

using CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public interface IPricer
    {
        decimal CalculatePrice(string ticker, IList<decimal> componentPrices);
    }
}
