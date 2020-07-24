namespace Trakx.Common.Interfaces
{
    public static class ExternalAddressExtension
    {
        public static string GetId(this IExternalAddress externalAddress)
        {
            return $"{externalAddress.CurrencySymbol}|{externalAddress.Address}";
        }

        public static string GetExternalAddressId(string currencySymbol, string address)
        {
            return $"{currencySymbol}|{address}";
        }
    }
}
