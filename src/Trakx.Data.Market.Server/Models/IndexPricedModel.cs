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
                    .ComponentQuantities.Select(ComponentModel.FromIComponent)
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
}


