using System.Text.Encodings.Web;

namespace Trakx.Data.Market.Common.Extensions
{
    public static class StringExtensions
    {
        public static string UrlEncode(this string rawString)
        {
            return UrlEncoder.Default.Encode(rawString);
        }
    }
}
