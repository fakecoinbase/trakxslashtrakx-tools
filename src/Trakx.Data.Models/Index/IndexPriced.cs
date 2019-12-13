using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Trakx.Data.Models.Index
{
    public class IndexPriced : IndexDefinition
    {
        public IndexPriced() { }

        public IndexPriced(IndexDefinition definition, 
            Dictionary<string, ComponentValuation> componentValuations)
        {
            if (definition.ComponentDefinitions.Any(c => !componentValuations.ContainsKey(c.Symbol)))
            {
                throw new InvalidDataException($"{nameof(componentValuations)} should contain valuations for all components of {definition.Symbol}.");
            }

            if (componentValuations.Select(c => c.Value.QuoteCurrency).Distinct().Count() > 1)
            {
                throw new InvalidDataException($"{nameof(componentValuations)} should all be quoted in the same currency.");
            }

            Symbol = definition.Symbol;
            Description = definition.Description;
            InitialValuation = definition.InitialValuation;
            Name = definition.Name;
            NaturalUnit = definition.NaturalUnit;
            ComponentDefinitions = definition.ComponentDefinitions;

            var naturalUnitScalingFactor = (decimal)Math.Pow(10, definition.NaturalUnit);
            var netAssetValue = componentValuations.Values.Sum(v => v.Value) / naturalUnitScalingFactor;
            CurrentValuation = new IndexValuation 
            {
                NetAssetValue = netAssetValue,
                ComponentWeights = componentValuations.ToDictionary(c => c.Key, c => c.Value.Value / netAssetValue),
                QuoteCurrency = componentValuations.First().Value.QuoteCurrency,
                TimeStamp = componentValuations.Max(c => c.Value.TimeStamp),
                ComponentValuations = componentValuations
            };
        }

        public IndexValuation CurrentValuation { get; set; }

        public new static readonly IndexPriced Default = new IndexPriced();
    }
}