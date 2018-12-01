using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Trakx.MarketData.Feeds.Common.Converters;
using Trakx.MarketData.Feeds.Common.Models.CryptoCompare;

namespace Trakx.MarketData.Feeds.Models.CryptoCompare
{
    public partial class CryptoCompareResponse : ICryptoCompareResponse
    {
        [JsonProperty("Response")]
        public string Response { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Data")]
        [JsonConverter(typeof(ConcreteDictionaryValueConverter<string, ICoin, Coin>))]
        public IDictionary<string, ICoin> Data { get; set; }

        [JsonProperty("BaseImageUrl")]
        public Uri BaseImageUrl { get; set; }

        [JsonProperty("BaseLinkUrl")]
        public Uri BaseLinkUrl { get; set; }

        [JsonProperty("RateLimit")]
        public object RateLimit { get; set; }

        [JsonProperty("HasWarning")]
        public bool HasWarning { get; set; }

        [JsonProperty("Type")]
        public long Type { get; set; }
    }
}