using System.Collections.Generic;

using CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public interface IResponseBuilder
    {
        PriceSingleResponse CalculatePriceSingleResponse(string ticker, PriceMultiResponse componentPricesResponse);
        PriceHistoricalReponse CalculatePriceHistoricalResponse(string ticker, IList<PriceHistoricalReponse> responses);
        PriceMultiFullResponse CalculatePriceMultiFullResponse(Dictionary<string, IList<string>> symbolsByTracker, PriceMultiFullResponse componentsPriceMultiFullResponse);
    }
}