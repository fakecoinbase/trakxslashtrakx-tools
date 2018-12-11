using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trakx.MarketData.Feeds.Models.CoinMarketCap
{
    public class HistoricalRecord
    {
        public string RecordCurrency { get; }
        public DateTime TimeStamp { get; }
        public int Rank { get; }
        public string Symbol { get; }
        public decimal MarketCap { get; }
        public decimal Price { get; }
        public uint CirculatingSupply { get; }
        public decimal Volume24h { get; }
        public decimal Change1h { get; }
        public decimal Change1d { get; }
        public decimal Change1w { get; }

        public HistoricalRecord(string rawHtml)
        {

        }
    }
}
