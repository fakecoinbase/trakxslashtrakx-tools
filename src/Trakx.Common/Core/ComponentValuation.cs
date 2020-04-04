using System;
using System.Diagnostics;
using Trakx.Common.Interfaces.Index;

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
        public void SetWeightFromTotalValue(decimal totalIndexValue)
        {
            Weight = (double)this.GetWeightFromTotalValue(totalIndexValue);
        }

        #endregion
    }
}