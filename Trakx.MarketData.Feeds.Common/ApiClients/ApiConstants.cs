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
            public const string MarketCapLatest = "v1/cryptocurrency/listings/latest";
            public const string MarketCapHistorical = "v1/cryptocurrency/listings/historical";

            public const string LimitParam = "limit";
            public const string TimeStampParam = "timestamp";
        }

        public static class CryptoCompare
        {
            public const string HttpClientName = "CryptoCompare";
            public const string SandboxEndpoint = @"https://min-api.cryptocompare.com/";
            public const string ApiKeyParam = "api_key";
            public const string ApiKey = "5f95e17ff4599da5bc6f4b309c2e0b27d3a73ddfaba843a63be66be7ebc3e79e";
            public const string ApiKeyHeader = "authorisation";
            public const string ApiKeyHeaderValue = "Apikey " + ApiKey;

            public const string AllCoins = "data/all/coinlist";
            public const string AllExchanges = "data/all/exchanges";
            public const string TopMarketCap = "data/top/mktcapfull";
            public const string TopTotalVol = "data/top/totalvol";
            public const string TopVolumes = "data/top/volumes";
            public const string TopPairs = "data/top/pairs";
            public const string Price = "data/price";
            public const string PriceHistorical = "data/pricehistorical";
            public const string PriceMultifull = "data/pricemultifull";
        }

    }
}
