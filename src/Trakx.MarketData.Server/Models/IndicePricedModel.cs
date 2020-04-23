using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ardalis.GuardClauses;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.MarketData.Server.Models
{
    public partial class IndicePricedModel
    {
        public IndiceValuationModel CurrentValuation { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Address { get; set; }
        public List<ComponentModel> ComponentDefinitions { get; set; }
        public IndiceValuationModel InitialValuation { get; set; }
        public long NaturalUnit { get; set; }
        public DateTime? CreationDate { get; set; }

        public static IndicePricedModel FromIndiceValuations(IIndiceValuation initialValuation,
            IIndiceValuation currentValuation)
        {
            Guard.Against.Default(initialValuation, nameof(initialValuation));
            Guard.Against.Default(currentValuation, nameof(currentValuation));

            Debug.Assert(ValuationsAreInTheSameCurrency(initialValuation, currentValuation),
                "All underlying component valuations should be quoted in the same currency.");

            var indicePriced = new IndicePricedModel()
            {
                Symbol = initialValuation.IndiceComposition.IndiceDefinition.Symbol.ToUpper(),
                Description = initialValuation.IndiceComposition.IndiceDefinition.Description,
                NaturalUnit = initialValuation.IndiceComposition.IndiceDefinition.NaturalUnit,
                Name = initialValuation.IndiceComposition.IndiceDefinition.Name,
                Address = initialValuation.IndiceComposition.IndiceDefinition.Address,
                InitialValuation = IndiceValuationModel.FromIIndiceValuation(initialValuation),
                CurrentValuation = IndiceValuationModel.FromIIndiceValuation(currentValuation),
                CreationDate = currentValuation.IndiceComposition.IndiceDefinition.CreationDate,
                ComponentDefinitions = currentValuation.IndiceComposition
                    .ComponentQuantities.Select(ComponentModel.FromIComponent)
                    .ToList()
            };
            return indicePriced;
        }

        private static bool ValuationsAreInTheSameCurrency(IIndiceValuation initialValuation, IIndiceValuation currentValuation)
        {
            return initialValuation.IsValid() 
                   && currentValuation.IsValid() 
                   && initialValuation.QuoteCurrency == currentValuation.QuoteCurrency;
        }
    }
}


