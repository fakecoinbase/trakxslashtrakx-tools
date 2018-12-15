using System;
using System.Linq;

namespace Trakx.MarketData.Feeds.Tests.ApiClients
{
    public class HistoricalMarketCapRecord
    {
        //setters are only here because of CsvHelper
        public DateTime TimeStamp { get; set; }
        public int Rank { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal? MarketCapUsd { get; set; }
        public decimal? MarketCapBtc { get; set; }
        public decimal? PriceUsd { get; set; }
        public decimal? PriceBtc { get; set; }
        public decimal? CirculatingSupply { get; set; }
        public decimal? Volume24HUsd { get; set; }
        public decimal? Volume24HBtc { get; set; }
        public decimal? Change1H { get; set; }
        public decimal? Change1D { get; set; }
        public decimal? Change1W { get; set; }

        public HistoricalMarketCapRecord(
            DateTime timeStamp,
            int rank,
            string currencyName,
            string currencySymbol,
            decimal? marketCapUsd,
            decimal? marketCapBtc,
            decimal? priceUsd,
            decimal? priceBtc,
            decimal? circulatingSupply,
            decimal? volume24HUsd,
            decimal? volume24HBtc,
            decimal? change1H,
            decimal? change1D,
            decimal? change1W)
        {
            TimeStamp = timeStamp;
            Rank = rank;
            CurrencyName = currencyName;
            CurrencySymbol = currencySymbol;
            MarketCapUsd = marketCapUsd;
            MarketCapBtc = marketCapBtc;
            PriceUsd = priceUsd;
            PriceBtc = priceBtc;
            CirculatingSupply = circulatingSupply;
            Volume24HUsd = volume24HUsd;
            Volume24HBtc = volume24HBtc;
            Change1H = change1H;
            Change1D = change1D;
            Change1W = change1W;
        }


        public static string GetCsvHeaders()
        {
            var headers = new []
                  {
                      nameof(TimeStamp),
                      nameof(Rank),
                      nameof(CurrencyName),
                      nameof(CurrencySymbol),
                      nameof(MarketCapUsd),
                      nameof(MarketCapBtc),
                      nameof(PriceUsd),
                      nameof(PriceBtc),
                      nameof(CirculatingSupply),
                      nameof(Volume24HUsd),
                      nameof(Volume24HBtc),
                      nameof(Change1H),
                      nameof(Change1D),
                      nameof(Change1W),
                  };
            return string.Join(",", headers.Select(AddQuotes));
        }

        public string ToCsv()
        {
            var values = new[]
              {
                  TimeStamp.ToString(),
                  Rank.ToString(),
                  CurrencyName,
                  CurrencySymbol,
                  MarketCapUsd.ToString(),
                  MarketCapBtc.ToString(),
                  PriceUsd.ToString(),
                  PriceBtc.ToString(),
                  CirculatingSupply.ToString(),
                  Volume24HUsd.ToString(),
                  Volume24HBtc.ToString(),
                  Change1H.ToString(),
                  Change1D.ToString(),
                  Change1W.ToString(),
              };
            return string.Join(",", values.Select(AddQuotes));
        }


        private static string AddQuotes(string unquoted)
        {
            return $"\"{unquoted}\"";
        }


    }
}
