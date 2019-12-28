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
    public interface IIndexDefinition<TComponentDefinition> 
        where TComponentDefinition : IComponentDefinition
    {
        /// <summary>
        /// Symbol of the token associated with this index (ex: L1MKC005).
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// Name (long) given to the index (ex: Top 5 Market Cap)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A brief explanation of the index and the choice of components it contains.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// If the index was created, the address at which the corresponding smart contract
        /// can be found on chain.
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// List of the components contained in the index.
        /// </summary>
        List<TComponentDefinition> ComponentDefinitions { get; set; }

        /// <summary>
        /// Net Asset Value of the index at creation time.
        /// </summary>
        IndexValuation InitialValuation { get; set; }

        /// <summary>
        /// Expressed as a power of 10, it represents the minimal amount of the token
        /// representing the index that one can buy. 
        /// </summary>
        int NaturalUnit { get; set; }

        /// <summary>
        /// Date at which the index was created.
        /// </summary>
        DateTime? CreationDate { get; set; }
    }
}