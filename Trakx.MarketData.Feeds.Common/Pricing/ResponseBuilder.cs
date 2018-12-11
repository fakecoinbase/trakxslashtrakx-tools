using System;
using System.Collections.Generic;
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
    }
}