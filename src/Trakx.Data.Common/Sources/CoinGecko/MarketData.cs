using System;

namespace Trakx.Data.Common.Sources.CoinGecko
{
    public class MarketData
    {
        public string CoinId { get; set; }
        public DateTime AsOf { get; set; }
        public string QuoteCurrency { get; set; }
        public decimal? Price { get; set; }
        public decimal? Volume { get; set; }
        public decimal? MarketCap { get; set; }
    }
}