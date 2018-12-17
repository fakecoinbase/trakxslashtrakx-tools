using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Trakx.MarketData.Feeds.Common.Helpers
{
    public static class DictionaryExtensions
    {
        public static IReadOnlyDictionary<TKey, TValue> AsReadonly<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}
