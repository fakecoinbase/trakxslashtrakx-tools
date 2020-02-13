using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ardalis.GuardClauses;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Market.Server.Models
{
    public partial class IndexPricedModel
    {
        public IndexValuationModel CurrentValuation { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Address { get; set; }
        public List<ComponentModel> ComponentDefinitions { get; set; }
        public IndexValuationModel InitialValuation { get; set; }
        public long NaturalUnit { get; set; }
        public DateTime? CreationDate { get; set; }

        public static IndexPricedModel FromIndexValuations(IIndexValuation initialValuation,
            IIndexValuation currentValuation)
        {
            Guard.Against.Default(initialValuation, nameof(initialValuation));
            Guard.Against.Default(currentValuation, nameof(currentValuation));

            Debug.Assert(ValuationsAreInTheSameCurrency(initialValuation, currentValuation),
                "All underlying component valuations should be quoted in the same currency.");

            var indexPriced = new IndexPricedModel()
            {
                Symbol = initialValuation.IndexComposition.IndexDefinition.Symbol.ToUpper(),
                Description = initialValuation.IndexComposition.IndexDefinition.Description,
                NaturalUnit = initialValuation.IndexComposition.IndexDefinition.NaturalUnit,
                Name = initialValuation.IndexComposition.IndexDefinition.Name,
                Address = initialValuation.IndexComposition.IndexDefinition.Address,
                InitialValuation = IndexValuationModel.FromIIndexValuation(initialValuation),
                CurrentValuation = IndexValuationModel.FromIIndexValuation(currentValuation),
                CreationDate = currentValuation.IndexComposition.IndexDefinition.CreationDate,
                ComponentDefinitions = currentValuation.IndexComposition
                    .ComponentQuantities.Select(c => ComponentModel.FromIComponent(c))
                    .ToList()
            };
            return indexPriced;
        }

        private static bool ValuationsAreInTheSameCurrency(IIndexValuation initialValuation, IIndexValuation currentValuation)
        {
            return initialValuation.IsValid() 
                   && currentValuation.IsValid() 
                   && initialValuation.QuoteCurrency == currentValuation.QuoteCurrency;
        }
    }

    public partial class ComponentModel
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public long Decimals { get; set; }
        public decimal Quantity { get; set; }
        public string IconUrl { get; set; }

        public static ComponentModel FromIComponent(IComponentQuantity componentQuantity)
        {
            var result = new ComponentModel()
            {
                Address = componentQuantity.ComponentDefinition.Address,
                Decimals = componentQuantity.ComponentDefinition.Decimals,
                Name = componentQuantity.ComponentDefinition.Name,
                Symbol = componentQuantity.ComponentDefinition.Symbol.ToUpper(),
                Quantity = componentQuantity.Quantity
            };
            return result;
        }
    }

    public partial class ComponentValuationModel
    {
        public string QuoteCurrency { get; set; }
        public DateTime TimeStamp { get; set; }
        public decimal Price { get; set; }
        public decimal Value { get; set; }
        public double? Weight { get; set; }

        public static ComponentValuationModel FromIComponentValuation(IComponentValuation valuation)
        {
            var result = new ComponentValuationModel()
            {
                QuoteCurrency = valuation.QuoteCurrency,
                TimeStamp = valuation.TimeStamp,
                Price = valuation.Price,
                Value = valuation.Value,
                Weight = valuation.Weight,
            };
            return result;
        }
    }

    public partial class IndexValuationModel
    {
        public DateTime TimeStamp { get; set; }
        public string QuoteCurrency { get; set; }
        public decimal NetAssetValue { get; set; }
        public Dictionary<string, ComponentValuationModel> ValuationsBySymbol { get; set; }

        public static IndexValuationModel FromIIndexValuation(IIndexValuation valuation)
        {
            var result = new IndexValuationModel()
            {
                NetAssetValue = valuation.NetAssetValue,
                QuoteCurrency = valuation.QuoteCurrency,
                TimeStamp = valuation.TimeStamp,
                ValuationsBySymbol = valuation.ComponentValuations.ToDictionary(
                    v => v.ComponentQuantity.ComponentDefinition.Symbol,
                    v => ComponentValuationModel.FromIComponentValuation(v))
            };
            return result;
        }
    }
}


