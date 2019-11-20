using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Trakx.Data.Market.Common.Sources.Kaiko.DTOs
{
    public partial class InstrumentsResponse
    {
        [JsonPropertyName("result")] public string Result { get; set; }
        [JsonPropertyName("data")] public List<Instrument> Instruments { get; set; }
    }

    public partial class Instrument
    {
        [JsonPropertyName("exchange_code")]
        public string ExchangeCode { get; set; }

        [JsonPropertyName("class")]
        public string Class { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("base_asset")]
        public string BaseAsset { get; set; }

        [JsonPropertyName("quote_asset")]
        public string QuoteAsset { get; set; }

        [JsonPropertyName("kaiko_legacy_exchange_slug")]
        public string KaikoLegacyExchangeSlug { get; set; }

        [JsonPropertyName("kaiko_legacy_symbol")]
        public string KaikoLegacySymbol { get; set; }

        [JsonPropertyName("exchange_pair_code")]
        public string ExchangePairCode { get; set; }

        [JsonPropertyName("trade_start_time")]
        public DateTimeOffset? TradeStartTime { get; set; }

        [JsonPropertyName("trade_start_timestamp")]
        public long? TradeStartTimestamp { get; set; }

        [JsonPropertyName("trade_end_time")]
        public DateTimeOffset? TradeEndTime { get; set; }

        [JsonPropertyName("trade_end_timestamp")]
        public long? TradeEndTimestamp { get; set; }

        [JsonPropertyName("trade_count")]
        public long TradeCount { get; set; }

        [JsonPropertyName("trade_compressed_size")]
        public long TradeCompressedSize { get; set; }
    }
}
