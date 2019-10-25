using System;
using System.Collections.Generic;

namespace Trakx.MarketApi.DataSources.Kaiko.DTOs
{
    public partial class InstrumentsResponse
    {
        public string Result { get; set; }
        public List<Instrument> Data { get; set; }
    }

    public partial class Instrument
    {
        public string ExchangeCode { get; set; }
        public string Class { get; set; }
        public string Code { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public string KaikoLegacyExchangeSlug { get; set; }
        public string KaikoLegacySymbol { get; set; }
        public string ExchangePairCode { get; set; }
        public DateTimeOffset TradeStartTime { get; set; }
        public long TradeStartTimestamp { get; set; }
        public DateTimeOffset? TradeEndTime { get; set; }
        public long? TradeEndTimestamp { get; set; }
        public long TradeCount { get; set; }
        public long TradeCompressedSize { get; set; }
    }

}
