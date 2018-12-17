using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Trakx.MarketData.Feeds.Common.Converters
{
    public class ConcreteDictionaryKeyValueTypeConverter<TKeyInterface, TKeyImplementation, TValueInterface, TValueImplementation> : JsonConverter 
        where TKeyImplementation : TKeyInterface
        where TValueImplementation : TValueInterface
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var concrete = serializer.Deserialize<Dictionary<TKeyImplementation, TValueImplementation>>(reader);
            var interfaced = concrete.ToDictionary(p => (TKeyInterface)p.Key, p => (TValueInterface)p.Value);
            return interfaced;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    public class ConcreteDictionaryValueConverter<TKey, TValueInterface, TValueImplementation> : JsonConverter
        where TValueImplementation : TValueInterface
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var concrete = serializer.Deserialize<Dictionary<TKey, TValueImplementation>>(reader);
            var interfaced = concrete.ToDictionary(p => p.Key, p => (TValueInterface)p.Value);
            return interfaced;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    public class ConcreteDictionaryKeyConverter<TKeyInterface, TKeyImplementation, TValue> : JsonConverter
        where TKeyImplementation : TKeyInterface
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var concrete = serializer.Deserialize<Dictionary<TKeyImplementation, TValue>>(reader);
            var interfaced = concrete.ToDictionary(p => (TKeyInterface)p.Key, p => p.Value);
            return interfaced;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
