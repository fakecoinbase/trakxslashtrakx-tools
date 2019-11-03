using System;

namespace Trakx.Data.Market.Common.Sources.CoinGecko
{
    public partial class ExchangeDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long YearEstablished { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public Uri Image { get; set; }
        public bool? HasTradingIncentive { get; set; }
        public int TrustScore { get; set; }
        public int TrustScoreRank { get; set; }
        public double TradeVolume24HBtc { get; set; }
        public double TradeVolume24HBtcNormalized { get; set; }
    }

}
