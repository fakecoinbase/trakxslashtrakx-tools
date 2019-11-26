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

using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Messari.DTOs
{
    public partial class Supply
    {
        [JsonPropertyName("y_2050")]
        public double? Y2050 { get; set; }

        [JsonPropertyName("y_plus10")]
        public double? YPlus10 { get; set; }

        [JsonPropertyName("liquid")]
        public double? Liquid { get; set; }

        [JsonPropertyName("circulating")]
        public double Circulating { get; set; }

        [JsonPropertyName("y_2050_issued_percent")]
        public double? Y2050_IssuedPercent { get; set; }

        [JsonPropertyName("annual_inflation_percent")]
        public double? AnnualInflationPercent { get; set; }

        [JsonPropertyName("stock_to_flow")]
        public double? StockToFlow { get; set; }

        [JsonPropertyName("y_plus10_issued_percent")]
        public double? YPlus10IssuedPercent { get; set; }
    }
}