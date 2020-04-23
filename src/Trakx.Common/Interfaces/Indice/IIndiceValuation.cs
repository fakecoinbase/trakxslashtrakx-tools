using System;
using System.Collections.Generic;

namespace Trakx.Common.Interfaces.Indice
{
    /// <summary>
    /// Represents a valuation of the indice, for a given composition, and at
    /// a given point in time.
    /// </summary>
    public interface IIndiceValuation
    {
        /// <summary>
        /// Composition of the indice for which the valuation was
        /// calculated.
        /// </summary>
        IIndiceComposition IndiceComposition { get;  }

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
