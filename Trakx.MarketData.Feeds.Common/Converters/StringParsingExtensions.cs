namespace Trakx.MarketData.Feeds.Common.Converters
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
    }
}
