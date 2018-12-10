using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CryptoCompare;

using Trakx.MarketData.Feeds.Common.StaticData;

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
            var price = _pricer.CalculatePrice(ticker, componentPriceResponses.Select(r => r["USD"]).ToList());

            var response = new PriceSingleResponse(new Dictionary<string, decimal>(){{"USD", price}});
            return response;
        }
    }
}