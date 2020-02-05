using System;
using System.Diagnostics;
using Ardalis.GuardClauses;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Common.Core
{
    /// <inheritdoc />
    public class ComponentValuation : IComponentValuation
    {
        public ComponentValuation(IComponentQuantity componentQuantity, string quoteCurrency, decimal price, DateTime timeStamp)
        {
            ComponentQuantity = componentQuantity;
            QuoteCurrency = quoteCurrency.ToLower();
            Price = price;
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
        public decimal Value => Price * ComponentQuantity.Quantity;

        /// <inheritdoc />
        public double? Weight { get; private set; }

        /// <inheritdoc />
        public DateTime TimeStamp { get; }

        /// <inheritdoc />
        public double SetWeightFromTotalValue(decimal totalIndexValue)
        {
            Guard.Against.NegativeOrZero(totalIndexValue, nameof(totalIndexValue));
            Weight = (double)(Value / totalIndexValue);
            return Weight.Value;
        }

        #endregion
    }
}