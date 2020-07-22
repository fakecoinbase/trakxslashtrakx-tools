using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Trakx.Coinbase.Custody.Client.Converter
{
    public class SystemStringToDecimalConverter : System.Text.Json.Serialization.JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String) return reader.GetDecimal();
            var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            return Utf8Parser.TryParse(span, out decimal number, out var bytesConsumed) && span.Length == bytesConsumed
                ? number
                : decimal.TryParse(reader.GetString(), out number)
                    ? number
                    : reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class NewtonSoftStringToDecimalConverter : JsonConverter<decimal>
    {
        #region Overrides of JsonConverter<decimal>

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = decimal.Parse((string) reader.Value);
            return value;
        }

        #endregion
    }
}