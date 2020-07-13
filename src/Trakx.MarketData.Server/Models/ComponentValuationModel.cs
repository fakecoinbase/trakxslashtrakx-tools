using System;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.MarketData.Server.Models
{
    /// <summary>
    /// Information about the valuation of a given index component at some point in time.
    /// </summary>
    public class ComponentValuationModel
    {
        #nullable disable
        /// <inheritdoc />
        public ComponentValuationModel() { }
        #nullable restore

        /// <inheritdoc />
        public ComponentValuationModel(IComponentValuation valuation)
        {
            QuoteCurrency = valuation.QuoteCurrency == "usdc" ? "USD" : valuation.QuoteCurrency;
            TimeStamp = valuation.TimeStamp;
            Price = valuation.Price;
            PriceSource = valuation.PriceSource;
            Value = valuation.Value;
            Weight = valuation.Weight;
        }

        /// <summary>
        /// The currency in which the valuation is expressed.
        /// </summary>
        public string QuoteCurrency { get; set; }

        /// <summary>
        /// UTC DateTime Stamp as of which the valuation was calculated.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The price of a single unit of the component.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The data source used to retrieve the price of the component.
        /// </summary>
        public string PriceSource { get; set; }

        /// <summary>
        /// The total value of the component as part of its index. This is
        /// effectively <see cref="IComponentQuantity.Quantity"/> * <see cref="Price"/>.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// The weight of the <see cref="Value"/> of component relative to the one of
        /// all the other components.
        /// </summary>
        public double? Weight { get; set; }

    }
}