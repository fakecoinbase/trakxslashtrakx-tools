using System;
using Microsoft.Extensions.Configuration;

namespace Trakx.MarketData.Collector.CryptoCompare
{
    public interface IApiDetailsProvider
    {
        string ApiKey { get;}
        Uri WebSocketEndpoint { get;}
    }

    public class ApiDetailsProvider : IApiDetailsProvider
    {
        public Uri WebSocketEndpoint { get; }
        public string ApiKey { get; }
        
        public ApiDetailsProvider(IConfiguration configuration)
        {
            ApiKey = Environment.GetEnvironmentVariable("CRYPTOCOMPARE_API_KEY");
            WebSocketEndpoint = new Uri($"wss://streamer.cryptocompare.com/v2?api_key={ApiKey}");
        }

        public ApiDetailsProvider(string apiKey)
        {
            ApiKey = apiKey;
            WebSocketEndpoint = new Uri($"wss://streamer.cryptocompare.com/v2?api_key={ApiKey}");
        }
    }
}
