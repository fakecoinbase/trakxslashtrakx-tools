using System;
using System.ComponentModel;
using System.Globalization;

using Newtonsoft.Json;

namespace Trakx.MarketData.Feeds.Common.Converters
{
    public class NumericOrNaConverter<T> : JsonConverter
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

            return TryParse<T>(reader.Value.ToString());
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(string));
        }

        public static T TryParse<T>(string inValue)
        {
            if (inValue == Na) return default(T);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
        }
    }
}