using System.Collections.Generic;

namespace Trakx.Common.Sources.Messari
{
    internal static class Constants
    {
        public static readonly string ApiEndpoint = @"https://data.messari.io/api/v1/";

        public static Dictionary<string, string> SectorSymbolBySector { get; } = new Dictionary<string, string>()
        {
            {"Asset Management", "amg"},
            {"Centralized Exchanges", "cex"},
            {"Decentralized Exchanges", "dex"},
            {"Lending", "len"},
            {"Scaling", "sca"},
        };
    }
}
