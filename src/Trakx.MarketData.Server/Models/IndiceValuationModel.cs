using System;
using System.Collections.Generic;
using System.Linq;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.MarketData.Server.Models
{
    public partial class IndiceValuationModel
    {
        public DateTime TimeStamp { get; set; }
        public string QuoteCurrency { get; set; }
        public decimal NetAssetValue { get; set; }
        public Dictionary<string, ComponentValuationModel> ValuationsBySymbol { get; set; }

        public static IndiceValuationModel FromIIndiceValuation(IIndiceValuation valuation)
        {
            var result = new IndiceValuationModel()
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