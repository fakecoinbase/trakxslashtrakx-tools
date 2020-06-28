using System.Collections.Generic;

namespace Trakx.Common.Composition
{
    public interface IWeightCalculator
    {
        /// <summary>
        /// Calculates the values to assign to each component based on its weight,
        /// and the total value expected for the composition they represent.
        /// </summary>
        /// <param name="componentWeightsBySymbol">Relative weights of the components, indexed by symbol of the components.</param>
        /// <param name="nav">Net Asset Value of the composition formed by the components.</param>
        /// <returns>A dictionary of the individual values of the components, indexed by their names.</returns>
        Dictionary<string, decimal> CalculateUsdcValuesFromNavAndWeights(Dictionary<string, decimal> componentWeightsBySymbol, decimal nav);

        /// <summary>
        /// Calculates the individual weights of the components based on their
        /// individual values.
        /// </summary>
        /// <param name="componentValuesBySymbol">Value of each component, indexed by their symbols.</param>
        Dictionary<string, decimal> CalculateWeightsFromUsdcValues(Dictionary<string, decimal> componentValuesBySymbol);

        /// <summary>
        /// Equally distributes the weights of the components, with a given precision.
        /// </summary>
        /// <param name="componentSymbols">List of the symbols on which to distribute the weight.</param>
        /// <param name="precision">Maximum precision used for the division.</param>
        /// /// <returns>A dictionary of the individual weights of the components, equally distributed.</returns>
        Dictionary<string, decimal> DistributeWeights(List<string> componentSymbols, int precision = 6);
    }
}