using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Trakx.Coinbase.Custody.Client.Converter
{
    public class SystemStringToULongConverter : System.Text.Json.Serialization.JsonConverter<ulong>
    {
        public override ulong Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String) return reader.GetUInt64();
            var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            return Utf8Parser.TryParse(span, out ulong number, out var bytesConsumed) && span.Length == bytesConsumed
                ? number
                : ulong.TryParse(reader.GetString().Replace(".0", ""), out number)
                    ? number
                    : reader.GetUInt64();
        }

        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class NewtonSoftStringToULongConverter : JsonConverter<ulong>
    {
        #region Overrides of JsonConverter<decimal>

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, ulong value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        public override ulong ReadJson(JsonReader reader, Type objectType, ulong existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = ulong.Parse(((string) reader.Value).Replace(".0", ""));
            return value;
        }

        #endregion
    }
}