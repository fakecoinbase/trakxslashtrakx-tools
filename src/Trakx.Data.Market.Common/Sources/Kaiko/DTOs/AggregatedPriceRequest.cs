using System;
using System.Collections.Generic;

namespace Trakx.Data.Market.Common.Sources.Kaiko.DTOs
{
    public class AggregatedPriceRequest
    {
        public string DataVersion { get; set; }
        public string Commodity { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public long PageSize { get; set; }
        public string Interval { get; set; }
        public string QuoteAsset { get; set; }
        public string BaseAsset { get; set; }
        public List<string> Exchanges { get; set; }
        public bool Sources { get; set; }
        public List<string> Instruments { get; set; }
    }
}
