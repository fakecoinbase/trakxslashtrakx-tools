using System.Text.Json.Serialization;

namespace Trakx.Common.Sources.Messari.DTOs
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
        public double? Circulating { get; set; }

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