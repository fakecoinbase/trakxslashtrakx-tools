using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CryptoCompare;

using Humanizer;

using Trakx.MarketData.Feeds.Common.Helpers;
using Trakx.MarketData.Feeds.Common.Trackers;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public class ResponseBuilder : IResponseBuilder
    {
        private readonly IPricer _pricer;
        private readonly ITrackerFactory _trackerFactory;

        public ResponseBuilder(IPricer pricer, ITrackerFactory trackerFactory)
        {
            _pricer = pricer;
            _trackerFactory = trackerFactory;
        }

        /// <inheritdoc />
        public PriceSingleResponse CalculatePriceSingleResponse(string ticker, PriceMultiResponse componentPricesResponse)
        {
            var price = _pricer.CalculatePricesByCurrencies(ticker, componentPricesResponse);

            var response = new PriceSingleResponse(price);
            return response;
        }

        /// <inheritdoc />
        public PriceHistoricalReponse CalculatePriceHistoricalResponse(string ticker, IList<PriceHistoricalReponse> responses)
        {
            var combinedResponse = responses.ToDictionary(r => r.Keys.Single(), r => r[r.Keys.Single()]);

            var prices = _pricer.CalculatePricesByCurrencies(ticker, combinedResponse);

            var response = new PriceHistoricalReponse(
                new Dictionary<string, IReadOnlyDictionary<string, decimal>>()
                    {
                        {ticker, new ReadOnlyDictionary<string, decimal>(prices)}
                    });

            return response;
        }

        /// <inheritdoc />
        public PriceMultiFullResponse CalculatePriceMultiFullResponse(Dictionary<string, IList<string>> symbolsByTracker, PriceMultiFullResponse componentsPriceMultiFullResponse)
        {
            var toCurrencies = componentsPriceMultiFullResponse.Raw
                .Values.SelectMany(p => p.Keys).Distinct();
            
            var rawPricesByToCurrencyByTracker = new Dictionary<string, IReadOnlyDictionary< string, CoinFullAggregatedData>> ();
            foreach (var trackerTicker in symbolsByTracker.Keys)
            {
                var rawResponses = componentsPriceMultiFullResponse.Raw
                    .Where(r => symbolsByTracker[trackerTicker].Contains(r.Key))
                    .ToDictionary(r => r.Key, r => r.Value);

                var dataByTickerByToCurrency = toCurrencies.ToDictionary(
                    t => t,
                    t => rawResponses.ToDictionary(r => r.Key, r => r.Value[t]));

                var trackerDetailsByCurrency = toCurrencies.ToDictionary(c => c, c =>
                {
                    var mergedRawResponse = MergeCoinFullAggregatedDatas(
                        trackerTicker,
                        dataByTickerByToCurrency[c]);
                    return mergedRawResponse;
                }).AsReadonly();

                rawPricesByToCurrencyByTracker.Add(trackerTicker, trackerDetailsByCurrency);
            }
            
            var priceMultiFullRaw = new PriceMultiFullRaw(rawPricesByToCurrencyByTracker);

            var displayPricesByToCurrencyByTracker = rawPricesByToCurrencyByTracker.ToDictionary(
                p => p.Key,
                p => p.Value.ToDictionary(q => q.Key, q => GetCoinFullDataDisplay(p.Key, q.Key, q.Value)).AsReadonly());

            var priceMultiFullDisplay = new PriceMultiFullDisplay(displayPricesByToCurrencyByTracker);

            var response = new PriceMultiFullResponse()
               {
                   Display = priceMultiFullDisplay,
                   Raw = priceMultiFullRaw
               };

            return response;
        }

        /// <summary>
        /// TODO: if this proves to be too slow on large number of components think about only looping once rather than once per field
        /// </summary>
        private CoinFullAggregatedData MergeCoinFullAggregatedDatas(
            string ticker,
            IReadOnlyDictionary<string, CoinFullAggregatedData> componentsData)
        {
            var tracker = _trackerFactory.FromTicker(ticker);

            var leverage = tracker.Leverage;
            var datas = componentsData.Values.ToList();
            
            var result = new CoinFullAggregatedData()
            {
                MarketCap = null,
                Flags = componentsData.Values.Select(c => c.Flags).Distinct().First(),
                FromSymbol = ticker,
                ToSymbol = componentsData.Values.Select(c => c.ToSymbol).Distinct().Single(),

                Price = _pricer.LeveragedAverage(leverage, datas, c => c.Price),

                Change24Hour = _pricer.LeveragedAverage(leverage, datas, c => c.Change24Hour),
                ChangeDay = _pricer.LeveragedAverage(leverage, datas, c => c.ChangeDay),
                ChangePCT24Hour = _pricer.LeveragedAverage(leverage, datas, c => c.ChangePCT24Hour),
                ChangePCTDay = _pricer.LeveragedAverage(leverage, datas, c => c.ChangePCTDay),

                High24Hour = _pricer.LeveragedAverage(leverage, datas, c => c.High24Hour),
                HighDay = _pricer.LeveragedAverage(leverage, datas, c => c.HighDay),
                Low24Hour = _pricer.LeveragedAverage(leverage, datas, c => c.Low24Hour),
                LowDay = _pricer.LeveragedAverage(leverage, datas, c => c.LowDay),
                Open24Hour = _pricer.LeveragedAverage(leverage, datas, c => c.Open24Hour),
                OpenDay = _pricer.LeveragedAverage(leverage, datas, c => c.OpenDay),

                LastTradeId = null,
                LastUpdate = componentsData.Values.Select(c => c.LastUpdate).Max(),
                LastVolume = null,
                LastVolumeTo = null,
                TotalVolume24H = null,
                TotalVolume24HTo = null,
                Volume24Hour = null,
                Volume24HourTo = null,
                VolumeDay = null,
                VolumeDayTo = null,

                Market = string.Join(",", componentsData.Values.Select(c => c.Market).Distinct().ToList()),
                LastMarket = string.Join(",", componentsData.Values.Select(c => c.LastMarket).Distinct().ToList()),
                Type = componentsData.Values.Select(c => c.Type).Distinct().Single(),
            };
            return result;
        }

        private CoinFullAggregatedDataDisplay GetCoinFullDataDisplay(string ticker, string targetCurrencySymbol, CoinFullAggregatedData rawData)
        {
            var tracker = _trackerFactory.FromTicker(ticker);

            var result = new CoinFullAggregatedDataDisplay()
            {
                MarketCap = null,
                FromSymbol = ticker,
                ToSymbol = targetCurrencySymbol,

                Price = $"{targetCurrencySymbol} {((double)rawData.Price).ToMetric()}",

                Change24Hour = $"{targetCurrencySymbol} {((double)rawData.Change24Hour).ToMetric()}",
                ChangeDay = $"{targetCurrencySymbol} {((double)rawData.ChangeDay).ToMetric()}",
                ChangePCT24Hour = $"{rawData.ChangePCT24Hour:0.##}%",
                ChangePCTDay = $"{rawData.ChangePCTDay:0.##}%",

                High24Hour = rawData.High24Hour.HasValue ? $"{targetCurrencySymbol} {rawData.High24Hour.Value.ToMetric()}" : null,
                HighDay = rawData.HighDay.HasValue ? $"{targetCurrencySymbol} {rawData.HighDay.Value.ToMetric()}" : null,
                Low24Hour = rawData.Low24Hour.HasValue ? $"{targetCurrencySymbol} {rawData.Low24Hour.Value.ToMetric()}" : null,
                LowDay = rawData.LowDay.HasValue ? $"{targetCurrencySymbol} {rawData.LowDay.Value.ToMetric()}" : null,
                Open24Hour = rawData.Open24Hour.HasValue ? $"{targetCurrencySymbol} {rawData.Open24Hour.Value.ToMetric()}" : null,
                OpenDay = rawData.OpenDay.HasValue ? $"{targetCurrencySymbol} {rawData.OpenDay.Value.ToMetric()}" : null,

                LastTradeId = null,
                LastUpdate = rawData.LastUpdate.Humanize(),
                LastVolume = null,
                LastVolumeTo = null,
                TotalVolume24H = null,
                TotalVolume24HTo = null,
                Volume24Hour = null,
                Volume24HourTo = null,
                VolumeDay = null,
                VolumeDayTo = null,

                Market = rawData.Market,
                LastMarket = rawData.LastMarket,
                Supply = "∞",
            };
            return result;
        }
    }
}