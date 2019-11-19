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
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class TokenSaleStats
    {
        [JsonPropertyName("sale_proceeds_usd")]
        public long? SaleProceedsUsd { get; set; }

        [JsonPropertyName("sale_start_date")]
        public DateTimeOffset? SaleStartDate { get; set; }

        [JsonPropertyName("sale_end_date")]
        public DateTimeOffset? SaleEndDate { get; set; }

        [JsonPropertyName("roi_since_sale_usd_percent")]
        public double? RoiSinceSaleUsdPercent { get; set; }

        [JsonPropertyName("roi_since_sale_btc_percent")]
        public double? RoiSinceSaleBtcPercent { get; set; }

        [JsonPropertyName("roi_since_sale_eth_percent")]
        public object RoiSinceSaleEthPercent { get; set; }
    }
}