using System;
using System.Diagnostics;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Core
{
    /// <inheritdoc />
    public class ComponentValuation : IComponentValuation
    {
        public ComponentValuation(IComponentQuantity componentQuantity, 
            string quoteCurrency, decimal price, string priceSource, DateTime timeStamp)
        {
            ComponentQuantity = componentQuantity;
            QuoteCurrency = quoteCurrency.ToLower();
            Price = price;
            PriceSource = priceSource;
            TimeStamp = timeStamp;

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IComponentValuation

        /// <inheritdoc />
        public IComponentQuantity ComponentQuantity { get; }

        /// <inheritdoc />
        public string QuoteCurrency { get; }

        /// <inheritdoc />
        public decimal Price { get; }

        /// <inheritdoc />
        public string PriceSource { get; }

        /// <inheritdoc />
        public decimal Value => Price * ComponentQuantity.Quantity;

        /// <inheritdoc />
        public double? Weight { get; private set; }

        /// <inheritdoc />
        public DateTime TimeStamp { get; }

        /// <inheritdoc />
        public void SetWeightFromTotalValue(decimal totalIndiceValue)
        {
            Weight = (double)this.GetWeightFromTotalValue(totalIndiceValue);
        }

        #endregion
    }
}