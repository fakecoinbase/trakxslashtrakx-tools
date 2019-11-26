using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Serialisation.Converters
{
    public class StringLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = long.Parse(reader.GetString());
            return value;
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
