using System;
using System.Collections.Generic;
using System.Linq;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Market.Server.Models
{
    public partial class IndexValuationModel
    {
        public DateTime TimeStamp { get; set; }
        public string QuoteCurrency { get; set; }
        public decimal NetAssetValue { get; set; }
        public Dictionary<string, ComponentValuationModel> ValuationsBySymbol { get; set; }

        public static IndexValuationModel FromIIndexValuation(IIndexValuation valuation)
        {
            var result = new IndexValuationModel()
            {
                NetAssetValue = valuation.NetAssetValue,
                QuoteCurrency = valuation.QuoteCurrency == "usdc" ? "USD" : valuation.QuoteCurrency,
                TimeStamp = valuation.TimeStamp,
                ValuationsBySymbol = valuation.ComponentValuations.ToDictionary(
                    v => v.ComponentQuantity.ComponentDefinition.Symbol.ToUpper(),
                    v => ComponentValuationModel.FromIComponentValuation(v))
            };
            return result;
        }
    }
}