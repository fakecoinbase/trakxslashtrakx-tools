using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ardalis.GuardClauses;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Common.Core
{
    /// <inheritdoc />
    public class IndexValuation : IIndexValuation
    {
        public IndexValuation(IIndexComposition composition, 
            List<IComponentValuation> componentValuations, 
            DateTime timeStamp)
        {
            Guard.Against.Default(composition, nameof(composition));
            Guard.Against.Default(componentValuations, nameof(componentValuations));
            Guard.Against.Default(timeStamp, nameof(timeStamp));

            IndexComposition = composition;
            ComponentValuations = componentValuations;
            TimeStamp = timeStamp;
            QuoteCurrency = componentValuations.First().QuoteCurrency;
            NetAssetValue = componentValuations.Sum(v => v.Value);
            componentValuations.ForEach(v => v.SetWeightFromTotalValue(NetAssetValue));

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IIndexValuation

        /// <inheritdoc />
        public IIndexComposition IndexComposition { get; }

        /// <inheritdoc />
        public List<IComponentValuation> ComponentValuations { get; }

        /// <inheritdoc />
        public DateTime TimeStamp { get; }

        /// <inheritdoc />
        public string QuoteCurrency { get; }

        /// <inheritdoc />
        public decimal NetAssetValue { get; }

        #endregion
    }
}