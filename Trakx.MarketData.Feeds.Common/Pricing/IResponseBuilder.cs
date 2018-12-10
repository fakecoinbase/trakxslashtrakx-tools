using System.Collections.Generic;

using CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public interface IResponseBuilder
    {
        PriceSingleResponse CalculatePriceSingleResponse(string ticker, IList<PriceSingleResponse> componentPriceResponses);
    }
}