using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trakx.Data.Common.Core;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Common.Composition
{
    public class IndexCompositionCalculator
    {
        private static decimal CalculateUnscaledComponentQuantity(int decimals, decimal price, decimal targetWeight,
            decimal targetIndexPrice, uint indexNaturalUnit)
        {
            var targetQuantity = targetWeight * targetIndexPrice / price * (decimal)Math.Pow(10, decimals + indexNaturalUnit - 18);
            return targetQuantity;
        }
        
        public static IComponentQuantity CalculateQuantity(IIndexDefinition indexDefinition,
            IComponentDefinition componentDefinition, 
            decimal price,
            decimal targetIndexPrice)
        {
            var targetWeight = indexDefinition.ComponentWeights.Single(c =>
                c.ComponentDefinition.Address.Equals(componentDefinition.Address,
                    StringComparison.InvariantCultureIgnoreCase)).Weight;

            var unscaled = CalculateUnscaledComponentQuantity(
                componentDefinition.Decimals, price,
                targetWeight,
                targetIndexPrice, indexDefinition.NaturalUnit);

            var quantity = new ComponentQuantity(componentDefinition, unscaled, indexDefinition.NaturalUnit);
            return quantity;
        }

        public static IIndexComposition CalculateIndexComposition(IIndexDefinition indexDefinition, 
            Dictionary<IComponentDefinition, decimal> componentPrices, decimal targetIndexPrice, 
            uint version, DateTime? creationDate = default)
        {
            var quantities = componentPrices.Select(v => 
                    CalculateQuantity(indexDefinition, v.Key, v.Value, targetIndexPrice));

            var composition = new IndexComposition(indexDefinition, quantities.ToList(), version, creationDate ?? DateTime.UtcNow);

            return composition;
        }
    }
}