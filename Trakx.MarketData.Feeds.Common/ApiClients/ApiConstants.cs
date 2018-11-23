using System;
using System.Collections.Generic;
using System.Text;

namespace Trakx.MarketData.Feeds.Common.ApiClients
{
    public static class ApiConstants
    {
        public static class CoinMarketCap
        {
            public const string HttpClientName = "CoinMarketCap";
            public const string SandboxEndpoint = @"https://sandbox-api.coinmarketcap.com/";
            public const string ProEndpoint = @"https://pro-api.coinmarketcap.com/";
            public const string ApiKeyHeader = "X-CMC_PRO_API_KEY";
            public const string ApiKeySandbox = "c55a4c2c-1ec2-41de-bb7e-e1fdf9df028b";
            public const string LatestMarketCap = "v1/cryptocurrency/listings/latest";
        }

    }
}
