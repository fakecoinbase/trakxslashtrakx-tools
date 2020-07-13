using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Common.Interfaces.Indice;
using Trakx.Common.Pricing;

namespace Trakx.Common.Interfaces.Pricing
{
    public interface INavCalculator
    {
        /// <summary>
        /// Use this to get the Net Asset Value of an indice for a given composition.
        /// </summary>
        /// <param name="composition">The composition of the indice used to calculate the valuation.</param>
        /// <param name="asOf">Use <see cref="default"/> to get latest price. Only pass a date here for historical prices.</param>
        /// <param name="quoteCurrency">The currency in which the valuation should be returned.</param>
        /// <returns>The Net Asset Value associated to this composition of this indice.</returns>
        Task<decimal> CalculateNav(IIndiceComposition composition, 
            DateTime? asOf = default, 
            string quoteCurrency = Constants.DefaultQuoteCurrency);

        /// <summary>
        /// Use this to get info about the valuation of an indice for a given composition.
        /// </summary>
        /// <param name="composition">The composition of the indice used to calculate the valuation.</param>
        /// <param name="asOf">Use <see cref="default"/> to get latest price. Only pass a date here for historical prices.</param>
        /// <param name="quoteCurrency">The currency in which the valuation should be returned.</param>
        /// <returns>The full <see cref="IIndiceValuation"/> associated to this composition of this indice.</returns>
        Task<IIndiceValuation> GetIndiceValuation(IIndiceComposition composition, 
            DateTime? asOf = default, 
            string quoteCurrency = Constants.DefaultQuoteCurrency);

        /// <summary>
        /// Calculates composition valuations for several points in past time.
        /// </summary>
        /// <param name="composition">The composition for which the valuations will be returned.</param>
        /// <param name="startTime">Earliest time for which the valuations are requested.</param>
        /// <param name="period">Period with which the valuations will be calculated.</param>
        /// <param name="endTime">Latest time for which the valuations are requested.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        Task<IEnumerable<IIndiceValuation>> GetCompositionValuations(IIndiceComposition composition, DateTime startTime, Period period, 
            DateTime? endTime = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calculates indice valuations for several points in past time.
        /// </summary>
        /// <param name="definition">The index definition for which the valuations will be returned.
        /// For each point in time, the composition of the index will be the one that was trading at
        /// that moment.</param>
        /// <param name="startTime">Earliest time for which the valuations are requested.</param>
        /// <param name="period">Period with which the valuations will be calculated.</param>
        /// <param name="endTime">Latest time for which the valuations are requested.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        Task<IEnumerable<IIndiceValuation>> GetIndexValuations(IIndiceDefinition definition, DateTime startTime, Period period, 
            DateTime? endTime = default, CancellationToken cancellationToken = default);
    }
}