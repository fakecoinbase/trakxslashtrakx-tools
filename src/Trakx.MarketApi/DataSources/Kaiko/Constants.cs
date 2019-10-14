using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace Trakx.MarketApi.DataSources.Kaiko
{
    internal static class Constants
    {
        public static readonly string ApiHttpHeader = "X-Api-Key";
        public static readonly string ApiKey = "75d63ffdbc021f188e63fe15f12a881a";
        public static readonly string MarketDataEndpoint = @"https://eu.market-api.kaiko.io/v1/";
        public static readonly string ReferenceDataEndpoint = @"https://reference-data-api.kaiko.io/v1/";
        public static readonly string SuccessResponse = "success";

        public static List<string> TrustedExchanges = new List<string>
        {
            "bnce", "krkn", "bfnx", "kcon", "polo",
            "btrx", "upbt", "cbse", "huob", "stmp"
        };

        public static string TrustedExchangesAsCsv => string.Join(",", TrustedExchanges);
    }
}
