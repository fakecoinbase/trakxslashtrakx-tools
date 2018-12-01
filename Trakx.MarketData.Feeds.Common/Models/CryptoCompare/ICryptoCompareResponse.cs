using System;
using System.Collections.Generic;

namespace Trakx.MarketData.Feeds.Common.Models.CryptoCompare
{
    public interface ICryptoCompareResponse
    {
        Uri BaseImageUrl { get; set; }
        Uri BaseLinkUrl { get; set; }
        IDictionary<string, ICoin> Data { get; set; }
        bool HasWarning { get; set; }
        string Message { get; set; }
        object RateLimit { get; set; }
        string Response { get; set; }
        long Type { get; set; }
    }
}