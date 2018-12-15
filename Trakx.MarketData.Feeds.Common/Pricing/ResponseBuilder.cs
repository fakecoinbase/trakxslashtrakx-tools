using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CryptoCompare;

namespace Trakx.MarketData.Feeds.Common.Pricing
{
    public class ResponseBuilder : IResponseBuilder
    {
        private readonly IPricer _pricer;

        public ResponseBuilder(IPricer pricer)
        {
            _pricer = pricer;
        }

        public PriceSingleResponse CalculatePriceSingleResponse(string ticker, IList<PriceSingleResponse> componentPriceResponses)
        {
            var price = _pricer.CalculatePricesByCurrencies(ticker, componentPriceResponses.Cast<IReadOnlyDictionary<string, decimal>>().ToList());

            var response = new PriceSingleResponse(price);
            return response;
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
    }
}