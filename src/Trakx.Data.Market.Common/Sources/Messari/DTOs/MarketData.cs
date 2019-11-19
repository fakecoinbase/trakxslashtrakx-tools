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
    public partial class MarketData
    {
        [JsonPropertyName("price_usd")]
        public double PriceUsd { get; set; }

        [JsonPropertyName("price_btc")]
        public double PriceBtc { get; set; }

        [JsonPropertyName("volume_last_24_hours")]
        public double VolumeLast24_Hours { get; set; }

        [JsonPropertyName("real_volume_last_24_hours")]
        public double? RealVolumeLast24_Hours { get; set; }

        [JsonPropertyName("volume_last_24_hours_overstatement_multiple")]
        public double? VolumeLast24_HoursOverstatementMultiple { get; set; }

        [JsonPropertyName("percent_change_usd_last_24_hours")]
        public double PercentChangeUsdLast24_Hours { get; set; }

        [JsonPropertyName("percent_change_btc_last_24_hours")]
        public double PercentChangeBtcLast24_Hours { get; set; }

        [JsonPropertyName("ohlcv_last_1_hour")]
        public OhlcvLastHour OhlcvLast1_Hour { get; set; }

        [JsonPropertyName("ohlcv_last_24_hour")]
        public OhlcvLastHour OhlcvLast24_Hour { get; set; }

        [JsonPropertyName("last_trade_at")]
        public DateTimeOffset LastTradeAt { get; set; }
    }
}