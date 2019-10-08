using System;
using System.Collections.Generic;

namespace Trakx.MarketApi.DataSources.Kaiko.AggregatedPrice
{
    public partial class Response
    {
        public Query Query { get; set; }
        public DateTimeOffset Time { get; set; }
        public long Timestamp { get; set; }
        public List<AggregatedPrice> Data { get; set; }
        public string Result { get; set; }
        public Access Access { get; set; }
    }

    public partial class Access
    {
        public Range AccessRange { get; set; }
        public Range DataRange { get; set; }
    }

    public partial class Range
    {
        public object StartTimestamp { get; set; }
        public object EndTimestamp { get; set; }
    }

    public partial class AggregatedPrice
    {
        public long Timestamp { get; set; }
        public long Count { get; set; }
        public string Volume { get; set; }
        public string Price { get; set; }
        public List<Source> Sources { get; set; }
    }

    public partial class Source
    {
        public long Timestamp { get; set; }
        public long Count { get; set; }
        public string Volume { get; set; }
        public string Price { get; set; }
        public string ExchangeCode { get; set; }
        public string Class { get; set; }
        public string Code { get; set; }
    }

    public partial class Query
    {
        public string DataVersion { get; set; }
        public string Commodity { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public long PageSize { get; set; }
        public string Interval { get; set; }
        public string QuoteAsset { get; set; }
        public string BaseAsset { get; set; }
        public List<string> Exchanges { get; set; }
        public bool Sources { get; set; }
        public DateTimeOffset RequestTime { get; set; }
        public List<string> Instruments { get; set; }
        public long StartTimestamp { get; set; }
    }

}
