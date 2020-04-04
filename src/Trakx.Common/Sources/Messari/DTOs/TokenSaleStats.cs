using System;
using System.Text.Json.Serialization;

namespace Trakx.Data.Common.Sources.Messari.DTOs
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