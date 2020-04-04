using System;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Index;
using Trakx.Common.Pricing;

namespace Trakx.Common.Interfaces.Pricing
{
    public interface INavCalculator
    {
        /// <summary>
        /// Use this to get the Net Asset Value of an index for a given composition.
        /// </summary>
        /// <param name="composition">The composition of the index used to calculate the valuation.</param>
        /// <param name="asOf">Use <see cref="default"/> to get latest price. Only pass a date here for historical prices.</param>
        /// <param name="quoteCurrency">The currency in which the valuation should be returned.</param>
        /// <returns>The Net Asset Value associated to this composition of this index.</returns>
        Task<decimal> CalculateNav(IIndexComposition composition, 
            DateTime? asOf = default, 
            string quoteCurrency = Constants.DefaultQuoteCurrency);

        /// <summary>
        /// Use this to get info about the valuation of an index for a given composition.
        /// </summary>
        /// <param name="composition">The composition of the index used to calculate the valuation.</param>
        /// <param name="asOf">Use <see cref="default"/> to get latest price. Only pass a date here for historical prices.</param>
        /// <param name="quoteCurrency">The currency in which the valuation should be returned.</param>
        /// <returns>The full <see cref="IIndexValuation"/> associated to this composition of this index.</returns>
        Task<IIndexValuation> GetIndexValuation(IIndexComposition composition, 
            DateTime? asOf = default, 
            string quoteCurrency = Constants.DefaultQuoteCurrency);
    }
}