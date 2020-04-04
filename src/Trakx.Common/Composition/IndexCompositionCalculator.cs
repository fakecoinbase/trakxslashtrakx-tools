using System;
using System.Collections.Generic;
using System.Linq;
using Trakx.Common.Core;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Common.Composition
{
    public struct PriceAndTargetWeight
    {
        public PriceAndTargetWeight(decimal price, decimal targetWeight)
        {
            Price = price;
            TargetWeight = targetWeight;
        }

        public decimal Price { get; }
        public decimal TargetWeight { get; }
    }

    public static class IndexCompositionCalculator
    {
        private static decimal CalculateUnscaledComponentQuantity(ushort decimals, PriceAndTargetWeight priceAnWeight,
            decimal targetIndexPrice, ushort indexNaturalUnit)
        {
            var targetQuantity = (priceAnWeight.TargetWeight * targetIndexPrice / priceAnWeight.Price)
                .DescaleComponentQuantity(decimals, indexNaturalUnit);
            return targetQuantity;
        }
        
        public static IComponentQuantity CalculateQuantity(IIndexDefinition indexDefinition,
            IComponentDefinition componentDefinition, 
            PriceAndTargetWeight priceAndWeight,
            decimal targetIndexPrice)
        {
            var unscaled = CalculateUnscaledComponentQuantity(
                componentDefinition.Decimals, priceAndWeight,
                targetIndexPrice, indexDefinition.NaturalUnit);

            var quantity = new ComponentQuantity(componentDefinition, unscaled, indexDefinition.NaturalUnit);
            return quantity;
        }

        public static IIndexComposition CalculateIndexComposition(IIndexDefinition indexDefinition, 
            Dictionary<IComponentDefinition, PriceAndTargetWeight> componentPrices, decimal targetIndexPrice, 
            uint version, DateTime? creationDate = default)
        {
            var quantities = componentPrices.Select(v => 
                    CalculateQuantity(indexDefinition, v.Key, v.Value, targetIndexPrice));

            var composition = new IndexComposition(indexDefinition, 
                quantities.ToList(), version, 
                creationDate ?? DateTime.UtcNow, "");

            return composition;
        }
    }
}