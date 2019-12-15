﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Trakx.Data.Models.Index
{
    public class IndexPriced : IndexDefinition
    {
        public IndexPriced() { }

        public IndexPriced(IndexDefinition definition, 
            List<ComponentValuation> componentValuations)
        {
            if (definition.ComponentDefinitions.Any(c =>
            !componentValuations.Any(v => c.Symbol.Equals(v.Definition.Symbol))))
            {
                throw new InvalidDataException(
                    $"{nameof(componentValuations)} should contain valuations for all components of {definition.Symbol}.");
            }

            if (componentValuations.Select(c => c.QuoteCurrency).Distinct().Count() > 1)
            {
                throw new InvalidDataException($"{nameof(componentValuations)} should all be quoted in the same currency.");
            }

            Symbol = definition.Symbol;
            Description = definition.Description;
            InitialValuation = definition.InitialValuation;
            Name = definition.Name;
            NaturalUnit = definition.NaturalUnit;
            ComponentDefinitions = definition.ComponentDefinitions;

            CurrentValuation = new IndexValuation(componentValuations, definition.NaturalUnit);
        }

        public IndexValuation CurrentValuation { get; set; }

        public new static readonly IndexPriced Default = new IndexPriced();
    }
}