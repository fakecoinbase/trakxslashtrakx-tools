using System;
using System.Globalization;

namespace Trakx.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToIso8601(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime()
                .ToString("o", CultureInfo.InvariantCulture);
        }
    }
}
