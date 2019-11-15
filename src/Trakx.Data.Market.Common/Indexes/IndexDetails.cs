using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Indexes
{
    public partial class IndexDetails
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("symbol")] public string Symbol { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("components")] public List<Component> Components { get; set; }
        [JsonPropertyName("targetUsdPrice")] public double TargetUsdPrice { get; set; }
        
        [JsonPropertyName("naturalUnit")]
        [JsonConverter(typeof(JsonStringBigIntegerConverter))]
        public BigInteger NaturalUnit { get; set; }
        
        [JsonPropertyName("usdBidAsk")] public UsdBidAsk UsdBidAsk { get; set; }
    }

    public partial class Component
    {
        [JsonPropertyName("address")] public string Address { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("symbol")] public string Symbol { get; set; }

        [JsonPropertyName("decimals")] public int Decimals { get; set; }

        [JsonPropertyName("usdBidAsk")] public UsdBidAsk UsdBidAsk { get; set; }

        [JsonPropertyName("quantity")]
        [JsonConverter(typeof(JsonStringBigIntegerConverter))]
        public BigInteger Quantity { get; set; }

        [JsonPropertyName("usdWeightAtCreation")]
        public string UsdWeightAtCreation { get; set; }

        [JsonPropertyName("usdValueAtCreation")]
        public string UsdValueAtCreation { get; set; }
    }

    public partial class UsdBidAsk
    {
        [JsonPropertyName("bid")] public string Bid { get; set; }

        [JsonPropertyName("ask")] public string Ask { get; set; }
    }

    public sealed class JsonStringBigIntegerConverter : JsonConverter<BigInteger>
    {
        public override BigInteger Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();

            return BigInteger.Parse(value);
        }

        public override void Write(Utf8JsonWriter writer,
            BigInteger value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}