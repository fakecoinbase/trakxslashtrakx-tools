using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trakx.Persistence.Converters
{
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
