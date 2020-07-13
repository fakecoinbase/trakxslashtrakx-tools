using System;
using System.Collections.Generic;
using System.Linq;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.MarketData.Server.Models
{
    /// <summary>
    /// Information about the valuation of a given index and its components at some point in time.
    /// </summary>
    public class IndiceValuationModel
    {
        #nullable disable
        /// <inheritdoc />
        public IndiceValuationModel() {}
        #nullable restore

        /// <inheritdoc />
        public IndiceValuationModel(IIndiceValuation valuation)
        {
            NetAssetValue = valuation.NetAssetValue;
            //todo: remove this USD/USDC hack one day, but it needs to be done on the exchange too ⚠!
            QuoteCurrency = valuation.QuoteCurrency == "usdc" ? "USD" : valuation.QuoteCurrency;
            TimeStamp = valuation.TimeStamp;
            ValuationsBySymbol = valuation.ComponentValuations.ToDictionary(
                v => v.ComponentQuantity.ComponentDefinition.Symbol.ToUpper(),
                v => new ComponentValuationModel(v));
            CompositionSymbol = valuation.IndiceComposition.Symbol;
        }

        /// <summary>
        /// Symbol of the composition of the index that was used to calculate the valuation.
        /// </summary>
        public string CompositionSymbol { get; set; }

        /// <summary>
        /// UTC DateTime Stamp as of which the valuation was calculated.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The currency in which the valuation is expressed.
        /// </summary>
        public string QuoteCurrency { get; set; }

        /// <summary>
        /// The Net Asset Value of the index. It is the sum of the individual <see cref="ComponentValuationModel.Value"/>
        /// of all the components in the index.
        /// </summary>
        public decimal NetAssetValue { get; set; }

        /// <summary>
        /// Details about the valuations of the components inside the index, indexed by their symbols.
        /// </summary>
        public Dictionary<string, ComponentValuationModel> ValuationsBySymbol { get; set; }
    }
}