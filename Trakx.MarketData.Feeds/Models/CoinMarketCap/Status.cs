using System;

using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.Models.CoinMarketCap;

namespace Trakx.MarketData.Feeds.Models.CoinMarketCap
{
    public partial class Status : IStatus
    {
        public Status(DateTimeOffset timestamp, long errorCode, object errorMessage, long elapsed, long creditCount)
        {
            Timestamp = timestamp;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Elapsed = elapsed;
            CreditCount = creditCount;
        }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; }

        [JsonProperty("error_code")]
        public long ErrorCode { get; }

        [JsonProperty("error_message")]
        public object ErrorMessage { get; }

        [JsonProperty("elapsed")]
        public long Elapsed { get; }

        [JsonProperty("credit_count")]
        public long CreditCount { get; }
    }
}