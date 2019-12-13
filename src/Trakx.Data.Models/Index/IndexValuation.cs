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

namespace Trakx.Data.Models.Index
{
    public class IndexValuation
    {
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
        public decimal NetAssetValue { get; set; }

        /// <summary>
        /// Weights of each components inside the index, expressed as a percentage of
        /// the net asset value.
        /// </summary>
        public Dictionary<string, decimal> ComponentWeights { get; set; }

        /// <summary>
        /// Valuations of each components inside the index, indexed by <see cref="ComponentDefinition.Symbol"/>
        /// </summary>
        public Dictionary<string, ComponentValuation> ComponentValuations { get; set; }
    }
}