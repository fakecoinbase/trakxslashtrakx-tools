using System.Collections.Generic;
using System.Linq;

namespace Trakx.MarketData.Feeds.Common.Helpers
{
    public static class StringParsingExtensions
    {
        public static decimal? ToDecimalOrNull(this string value)
        {
            var result = decimal.TryParse(value, out decimal parsedValue) ? parsedValue : (decimal?)null;
            return result;
        }

        public static uint? ToUintOrNull(this string value)
        {
            var result = uint.TryParse(value, out uint parsedValue) ? parsedValue : (uint?)null;
            return result;
        }

        public static IList<string> FromCsvToDistinctNonNullOrWhiteSpaceUpperList(this string listAsCsv)
        {
            if(listAsCsv == null) return null;
            if(string.IsNullOrWhiteSpace(listAsCsv)) return new List<string>();
            var result = listAsCsv.Split(",")
                .Select(s => s.Trim().ToUpperInvariant())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();
            return result;
        }
    }
}
