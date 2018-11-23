using System;

using Newtonsoft.Json;

namespace Trakx.MarketData.Feeds.Common.Converters
{
    public class Uint32WithNaConverter : JsonConverter
    {
        private const string Na = "N/A";

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value == null ? Na : value.ToString());
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return uint.TryParse(reader.Value.ToString(), out uint result)
                       ? result : (uint?)null;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(string));
        }
    }
}