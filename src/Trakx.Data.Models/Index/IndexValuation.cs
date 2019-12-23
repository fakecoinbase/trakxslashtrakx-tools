#region LICENSE

// 
// Copyright (c) 2019 Catalyst Network
// 
// This file is part of Catalyst.Node <https://github.com/catalyst-network/Catalyst.Node>
// 
// Catalyst.Node is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Catalyst.Node is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Catalyst.Node. If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Trakx.Data.Models.Index
{
    public class IndexValuation
    {
        private Dictionary<string, decimal> _componentWeights;
        private Dictionary<string, FlatComponentValuation> _valuationsBySymbol;

        public IndexValuation()
        {
            
        }

        public IndexValuation(List<ComponentValuation> componentValuations)
        {
            NetAssetValue = componentValuations.Sum(v => v.Value);
            QuoteCurrency = componentValuations.First().QuoteCurrency;
            TimeStamp = componentValuations.Max(c => c.TimeStamp);
            ComponentValuations = componentValuations;
        }

        public IndexValuation(List<ComponentDefinition> componentDefinitions)
            : this(componentDefinitions.Select(d => d.InitialValuation).ToList()) { }

        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [JsonIgnore]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Date at which the valuation calculation was performed
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Currency in which the valuation is expressed.
        /// </summary>
        public string QuoteCurrency { get; set; }

        /// <summary>
        /// Sum of the individual values of all the components included in the basket.
        /// </summary>
        [Column(TypeName = "decimal(38, 18)")]
        public decimal NetAssetValue { get; set; }

        /// <summary>
        /// Weights of each components inside the index, expressed as a percentage of
        /// the net asset value.
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public Dictionary<string, decimal> ComponentWeights =>
            _componentWeights ?? (_componentWeights =
                ComponentValuations.ToDictionary(c => c.ComponentDefinition.Symbol, c => c.Value / NetAssetValue));

        /// <summary>
        /// Valuations of each components inside the index, indexed by <see cref="ComponentDefinition.Symbol"/>
        /// </summary>
        [JsonIgnore]
        public List<ComponentValuation> ComponentValuations { get; set; }

        /// <summary>
        /// Convenience property allowing access to components by their symbol.
        /// </summary>
        [NotMapped]
        public Dictionary<string, FlatComponentValuation> ValuationsBySymbol =>
            _valuationsBySymbol ?? (_valuationsBySymbol =
                ComponentValuations.ToDictionary(c => c.ComponentDefinition.Symbol,
                    c => new FlatComponentValuation
                        {
                            TimeStamp = c.TimeStamp,
                            QuoteCurrency = c.QuoteCurrency,
                            Price = c.Price,
                            Value = c.Value,
                            Weight = ComponentWeights[c.ComponentDefinition.Symbol]
                        }
                    ));

        public class FlatComponentValuation
        {
            public DateTime TimeStamp { get; set; }
            public string QuoteCurrency { get; set; }
            public decimal Price { get; set; }
            public decimal Value { get; set; }
            public decimal Weight { get; set; }
        }
    }
}