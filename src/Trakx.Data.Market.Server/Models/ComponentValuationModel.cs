using System;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Market.Server.Models
{
    public partial class ComponentValuationModel
    {
        public string QuoteCurrency { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal Price { get; set; }
        public string PriceSource { get; set; }
        public decimal Value { get; set; }
        public double? Weight { get; set; }

        public static ComponentValuationModel FromIComponentValuation(IComponentValuation valuation)
        {
            var result = new ComponentValuationModel()
            {
                QuoteCurrency = valuation.QuoteCurrency == "usdc" ? "USD" : valuation.QuoteCurrency,
                TimeStamp = valuation.TimeStamp,
                Price = valuation.Price,
                PriceSource = valuation.PriceSource,
                Value = valuation.Value,
                Weight = valuation.Weight,
            };
            return result;
        }
    }
}