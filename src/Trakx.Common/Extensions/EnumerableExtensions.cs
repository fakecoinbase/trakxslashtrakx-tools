using System.Collections.Generic;
using System.Linq;

namespace Trakx.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> IntersectMany<T>(this IEnumerable<IEnumerable<T>> enumOfEnums)
        {
            var ofEnums = enumOfEnums as List<IEnumerable<T>> ?? enumOfEnums.ToList();
            if (!ofEnums.Any()) return Enumerable.Empty<T>();
            var intersection = ofEnums
                .Skip(1)
                .Aggregate(
                    new HashSet<T>(ofEnums.First()),
                    (h, e) => { h.IntersectWith(e); return h; }
                );
            return intersection;
        }
    }
}