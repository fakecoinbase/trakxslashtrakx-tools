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

namespace Trakx.Data.Models.Index
{
    public interface IComponentDefinition
    {
        /// <summary>
        /// Address of the smart contract defining the ERC20 contract of the component.
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// Common (longer) name for the component (ex: Bitcoin, Ethereum, Litecoin).
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Symbol of the token associated with this component (ex: ETH, BTC, LTC).
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// Number of decimals by which 1 unit of the ERC20 can be divided.
        /// </summary>
        int Decimals { get; set; }

        /// <summary>
        /// Units of the component contained in each unit of the index containing it. This is
        /// always expressed in the smallest unit of the component's currency.
        /// </summary>
        ulong Quantity { get; set; }

        /// <summary>
        /// Valuation of the component at the time of creation
        /// </summary>
        ComponentValuation InitialValuation { get; set; }
    }
}