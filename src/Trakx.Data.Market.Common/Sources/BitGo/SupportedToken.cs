namespace Trakx.Data.Market.Common.Sources.BitGo
{
    public class SupportedToken
    {
        public string? Identifier { get; set; }
        public string? DigitalCurrency { get; set; }
        public string? Family { get; set; }
        public string? BitGoEnvironment { get; set; }
        public string? ReleaseStatus { get; set; }
    }
}