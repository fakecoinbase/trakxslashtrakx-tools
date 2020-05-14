using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Trakx.Contracts
{
    public static class ReflectionHelper
    {
        public static ReadOnlyDictionary<string, string> GetStaticStringPropertiesByNames<T>()
        {
            return new ReadOnlyDictionary<string, string>(
                typeof(T)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsInitOnly && fi.FieldType == typeof(string))
                    .ToDictionary(fi => fi.Name, fi => fi.GetValue(null).ToString()));
        }
    }
}