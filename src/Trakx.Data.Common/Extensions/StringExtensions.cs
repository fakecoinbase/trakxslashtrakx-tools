using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace Trakx.Data.Common.Extensions
{
    public static class StringExtensions
    {
        public static Regex EthereumAddressRegex = new Regex(@"(?<Prefix>0x)(?<Address>[A-F,a-f,0-9]{40})");

        public static string UrlEncode(this string rawString)
        {
            return UrlEncoder.Default.Encode(rawString);
        }

        public static bool IsValidEthereumAddress(this string address)
        {
            return EthereumAddressRegex.IsMatch(address);
        }
    }
}
