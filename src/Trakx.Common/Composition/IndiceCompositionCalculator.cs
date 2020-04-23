using System;
using System.Collections.Generic;
using System.Linq;
using Trakx.Common.Core;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Indice;

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

    public static class IndiceCompositionCalculator
    {
        private static decimal CalculateUnscaledComponentQuantity(ushort decimals, PriceAndTargetWeight priceAnWeight,
            decimal targetIndicePrice, ushort indiceNaturalUnit)
        {
            var targetQuantity = (priceAnWeight.TargetWeight * targetIndicePrice / priceAnWeight.Price)
                .DescaleComponentQuantity(decimals, indiceNaturalUnit);
            return targetQuantity;
        }
        
        public static IComponentQuantity CalculateQuantity(IIndiceDefinition indiceDefinition,
            IComponentDefinition componentDefinition, 
            PriceAndTargetWeight priceAndWeight,
            decimal targetIndicePrice)
        {
            var unscaled = CalculateUnscaledComponentQuantity(
                componentDefinition.Decimals, priceAndWeight,
                targetIndicePrice, indiceDefinition.NaturalUnit);

            var quantity = new ComponentQuantity(componentDefinition, unscaled, indiceDefinition.NaturalUnit);
            return quantity;
        }

        public static IIndiceComposition CalculateIndiceComposition(IIndiceDefinition indiceDefinition, 
            Dictionary<IComponentDefinition, PriceAndTargetWeight> componentPrices, decimal targetIndicePrice, 
            uint version, DateTime? creationDate = default)
        {
            var quantities = componentPrices.Select(v => 
                    CalculateQuantity(indiceDefinition, v.Key, v.Value, targetIndicePrice));

            var composition = new IndiceComposition(indiceDefinition, 
                quantities.ToList(), version, 
                creationDate ?? DateTime.UtcNow, "");

            return composition;
        }
    }
}