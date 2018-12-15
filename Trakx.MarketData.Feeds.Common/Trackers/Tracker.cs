using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CryptoCompare;

using Trakx.MarketData.Feeds.Common.StaticData;

namespace Trakx.MarketData.Feeds.Common.Trackers
{
    public class Tracker
    {
        private const string LeverageDirection = nameof(LeverageDirection);
        private const string LeverageAmplitude = nameof(LeverageAmplitude);
        private const string SymbolGroup = nameof(SymbolGroup);
        private const string BasketSizeGroup = nameof(BasketSizeGroup);
        private const string Long = "L";
        private const string Inverse = "I";

        protected static string tickerPattern = "^(?<" +
                                              LeverageDirection + $">({Inverse}|{Long}))(?<" +
                                              LeverageAmplitude + ">[\\d]{1})(?<" +
                                              SymbolGroup + ">(" + string.Join("|", TrackerSymbols.AllSymbols) + ")+)(?<" +
                                              BasketSizeGroup + ">[\\d]*)$";

        private static Regex _regex = new Regex(tickerPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string _ticker;
        public string Ticker => _ticker ?? (_ticker = ToString());
        public int Leverage { get; }
        public virtual string Symbol { get; }
        public virtual int BasketSize { get; }

        public Tracker(int leverage, string symbol, int basketSize)
        {
            Leverage = leverage;
            Symbol = symbol;
            BasketSize = basketSize;
        }

        public Tracker(string ticker)
        {
            var match = _regex.Matches(ticker).Single();

            _ticker = ticker;

            var sign = match.Groups[LeverageDirection].Value.ToUpperInvariant().Equals(Long) ? 1 : -1;
            var amplitude = int.Parse(match.Groups[LeverageAmplitude].Value);
            Leverage = sign * amplitude;
            Symbol = match.Groups[SymbolGroup].Value.ToUpperInvariant();
            var basketSizeString = match.Groups[BasketSizeGroup].Value;
            BasketSize = string.IsNullOrEmpty(basketSizeString) 
                             ? 1 : int.Parse(basketSizeString);
        }

        public Func<ICryptoCompareClient, Task<IList<string>>> ComponentExtractor
        {
            get
            {
                if (TrackerSymbols.AllSingleNameSymbols.Contains(Symbol))
                    return _ => Task.FromResult(new List<string>() { Symbol } as IList<string>);
                
                if (Symbol == TrackerSymbols.MarketCap)
                       return async c =>
                           {
                               var tops = await c.Tops.CoinFullDataByMarketCap("USD", Math.Max(BasketSize, 10));
                               var result = tops.Data.Take(BasketSize).Select(d => d.CoinInfo.Name).ToList();
                               return result;
                           };

                return _ => throw new NotImplementedException($"Unable to extract component tickers for symbol {Symbol}");
            }
        }

        public override string ToString()
        {
            var sign = Leverage < 0 ? Inverse : Long;
            var basketSize = TrackerSymbols.AllSingleNameSymbols.Contains(Symbol)
                                 ? String.Empty
                                 : BasketSize.ToString("000");

            var result = $"{sign}{Math.Abs(Leverage)}{Symbol}{basketSize}";
            return result;
        }
    }

    //todo : replace "ifs" in ComponentExtractor with type based logic 
    //public class SingleCurrencyTracker : Tracker
    //{
    //    public SingleCurrencyTracker(string ticker) : base(ticker)
    //    {
    //        if (BasketSize != 1) throw new InvalidCastException($"Ticker {ticker} is not valid as a {typeof(SingleCurrencyTracker)}, basket size is {BasketSize}.");
    //    }

    //    public SingleCurrencyTracker(int leverage, string symbol, int basketSize)
    //        : base(leverage, symbol, 1) { }
    //}
}
