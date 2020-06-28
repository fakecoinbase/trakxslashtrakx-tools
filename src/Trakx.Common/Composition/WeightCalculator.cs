using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Trakx.Common.Extensions;

namespace Trakx.Common.Composition
{
    public class WeightCalculator : IWeightCalculator
    {
        /// <inheritdoc />
        public Dictionary<string, decimal> CalculateUsdcValuesFromNavAndWeights(Dictionary<string, decimal> componentWeightsBySymbol, decimal nav)
        {
            Guard.Against.NullOrEmpty(componentWeightsBySymbol, nameof(componentWeightsBySymbol));
            Guard.Against.NegativeOrZero(nav, nameof(nav));

            return componentWeightsBySymbol.ToDictionary(p => p.Key, p => p.Value * nav);
        }

        /// <inheritdoc />
        public Dictionary<string, decimal> CalculateWeightsFromUsdcValues(Dictionary<string, decimal> componentValuesBySymbol)
        {
            Guard.Against.NullOrEmpty(componentValuesBySymbol, nameof(componentValuesBySymbol));
            var totalValue = componentValuesBySymbol.Sum(c => c.Value);
            
            var componentWeightsBySymbol = componentValuesBySymbol
                .ToDictionary(p => p.Key, p => totalValue == 0 ? 0 : FloorWithPrecision(p.Value / totalValue, 6));
            
            var remainder = 1m - componentWeightsBySymbol.Sum(p => p.Value);
            componentWeightsBySymbol[componentWeightsBySymbol.Keys.First()] += remainder;
            
            return componentWeightsBySymbol;
        }

        /// <inheritdoc />
        public Dictionary<string, decimal> DistributeWeights(List<string> componentSymbols, int precision = 6)
        {
            Guard.Against.NullOrEmpty(componentSymbols, nameof(componentSymbols));
            Guard.Against.Negative(precision, nameof(precision));

            var weight = FloorWithPrecision(1m / componentSymbols.Count, precision);
            var remainder = 1m - weight * (componentSymbols.Count - 1);
            
            var distributedWeights = componentSymbols.ToDictionary(s => s, s => weight);
            distributedWeights[componentSymbols.First()] = remainder;
            
            return distributedWeights;
        }

        /// <summary>
        /// It looks like .Net Standard doesn't have the good RoundingDirection available, so we do it manually.
        /// </summary>
        /// <param name="value">Value we want to floor.</param>
        /// <param name="precision">Number of decimals after which we want to truncate.</param>
        private decimal FloorWithPrecision(decimal value, int precision)
        {
            var precisionMultiplier = precision.AsAPowerOf10();
            return Math.Floor(precisionMultiplier * value) / precisionMultiplier;
        }
    }
}
