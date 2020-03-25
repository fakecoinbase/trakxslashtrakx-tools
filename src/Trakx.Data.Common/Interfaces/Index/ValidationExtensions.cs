using System.Linq;
using Ardalis.GuardClauses;

namespace Trakx.Data.Common.Interfaces.Index
{
    /// <summary>
    /// Convenience methods to ensure the various object involved in representing the indexes are valid.
    /// </summary>
    public static class ValidationExtensions
    {
        public static bool IsValid(this IComponentDefinition definition)
        {
            Guard.Against.Default(definition, nameof(definition));
            Guard.Against.NullOrWhiteSpace(definition.Address, nameof(definition.Address));
            Guard.Against.NullOrWhiteSpace(definition.Name, nameof(definition.Name));
            Guard.Against.NullOrWhiteSpace(definition.Symbol, nameof(definition.Symbol));
            Guard.Against.NullOrWhiteSpace(definition.CoinGeckoId, nameof(definition.CoinGeckoId));
            Guard.Against.OutOfRange(definition.Decimals, nameof(definition.Decimals), 0, 18);
            return true;
        }

        public static bool IsValid(this IComponentQuantity componentQuantity)
        {
            Guard.Against.Default(componentQuantity, nameof(componentQuantity));
            Guard.Against.Default(componentQuantity.ComponentDefinition, nameof(componentQuantity.ComponentDefinition));
            return componentQuantity.ComponentDefinition.IsValid();
        }

        public static bool IsValid(this IComponentWeight componentWeight)
        {
            Guard.Against.Default(componentWeight, nameof(componentWeight));
            Guard.Against.Default(componentWeight.ComponentDefinition, nameof(componentWeight.ComponentDefinition));
            return componentWeight.ComponentDefinition.IsValid();
        }

        public static bool IsValid(this IComponentValuation valuation)
        {
            Guard.Against.Default(valuation, nameof(valuation));
            Guard.Against.Negative(valuation.Price, nameof(valuation.Price));
            Guard.Against.NullOrWhiteSpace(valuation.QuoteCurrency, nameof(valuation.QuoteCurrency));
            Guard.Against.Default(valuation.TimeStamp, nameof(valuation.TimeStamp));
            Guard.Against.Negative(valuation.Value, nameof(valuation.Value));
            return valuation.ComponentQuantity.IsValid();
        }

        public static bool IsValid(this IIndexDefinition definition)
        {
            Guard.Against.Default(definition, nameof(definition));
            Guard.Against.NullOrWhiteSpace(definition.Name, nameof(definition.Name));
            Guard.Against.NullOrWhiteSpace(definition.Symbol, nameof(definition.Symbol));
            Guard.Against.NullOrWhiteSpace(definition.Description, nameof(definition.Description));
            Guard.Against.OutOfRange(definition.NaturalUnit, nameof(definition.NaturalUnit), 0, 50);
            return true;
        }

        public static bool IsValid(this IIndexComposition composition)
        {
            Guard.Against.Default(composition, nameof(composition));
            Guard.Against.Default(composition.ComponentQuantities, nameof(composition.ComponentQuantities));
            return composition.IndexDefinition.IsValid();
        }

        public static bool IsValid(this IIndexValuation valuation)
        {
            Guard.Against.Default(valuation, nameof(valuation));
            Guard.Against.Negative(valuation.NetAssetValue, nameof(valuation.NetAssetValue));
            Guard.Against.Default(valuation.TimeStamp, nameof(valuation.TimeStamp));
            Guard.Against.Default(valuation.IndexComposition, nameof(valuation.IndexComposition));

            Guard.Against.Default(valuation.ComponentValuations, nameof(valuation.ComponentValuations));

            var isCompositionValid = valuation.IndexComposition.IsValid();
            if (!isCompositionValid) return false;

            var areAllAndOnlyCompositionComponentsValued = valuation.IndexComposition.ComponentQuantities.Count == valuation.ComponentValuations.Count
              && !valuation.IndexComposition.ComponentQuantities.Select(q => q.ComponentDefinition.Address)
                  .Except(valuation.ComponentValuations.Select(c => c.ComponentQuantity.ComponentDefinition.Address)).Any();
            
            if (!areAllAndOnlyCompositionComponentsValued) return false;

            var allValuationsAreInTheCorrectCurrency = true;

            foreach (var componentValuation in valuation.ComponentValuations)
            {
                allValuationsAreInTheCorrectCurrency &= valuation.QuoteCurrency == componentValuation.QuoteCurrency;
            }

            return allValuationsAreInTheCorrectCurrency;
        }
    }
}
