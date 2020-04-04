using System;
using System.Collections.Generic;

namespace Trakx.Common.Interfaces.Index
{
    /// <summary>
    /// Represents a valuation of the index, for a given composition, and at
    /// a given point in time.
    /// </summary>
    public interface IIndexValuation
    {
        /// <summary>
        /// Composition of the index for which the valuation was
        /// calculated.
        /// </summary>
        IIndexComposition IndexComposition { get;  }

        /// <summary>
        /// Valuations of the components that have been used to
        /// calculate the current <see cref="NetAssetValue"/>.
        /// </summary>
        List<IComponentValuation> ComponentValuations { get; }

        /// <summary>
        /// Date at which the valuation calculation was performed.
        /// </summary>
        DateTime TimeStamp { get; }

        /// <summary>
        /// Currency in which the valuation is expressed.
        /// </summary>
        string QuoteCurrency { get; }

        /// <summary>
        /// Sum of the individual values of all the components included in the basket.
        /// </summary>
        decimal NetAssetValue { get; }
    }
}
