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

        public static class CryptoCompare
        {
            public const string HttpClientName = "CryptoCompare";
            public const string SandboxEndpoint = @"https://min-api.cryptocompare.com/";
            public const string ApiKeyParam = "api_key";
            public const string ApiKey = "5f95e17ff4599da5bc6f4b309c2e0b27d3a73ddfaba843a63be66be7ebc3e79e";
            public const string AllCoins = "data/all/coinlist";
        }

    }
}
