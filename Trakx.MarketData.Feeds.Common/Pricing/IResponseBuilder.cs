﻿using System.Collections.Generic;

using CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public interface IResponseBuilder
    {
        PriceSingleResponse CalculatePriceSingleResponse(string ticker, PriceMultiResponse componentPricesResponse);
        PriceHistoricalReponse CalculatePriceHistoricalResponse(string ticker, IList<PriceHistoricalReponse> responses);
        PriceMultiFullResponse CalculatePriceMultiFullResponse(Dictionary<string, IList<string>> symbolsByTracker, PriceMultiFullResponse componentsPriceMultiFullResponse);
        TopResponse CalculateTopPairResponse(string trackerSymbol, IReadOnlyDictionary<string, decimal> trackerPrices, TopResponse btcPairs);
        TopVolume24HResponse CalculateTopVolumesResponse(CoinListResponse trackers, PriceMultiFullResponse prices, TopVolume24HResponse baseResponse);
    }
}