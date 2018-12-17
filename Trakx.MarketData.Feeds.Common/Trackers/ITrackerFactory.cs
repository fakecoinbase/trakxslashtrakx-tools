using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Trakx.MarketData.Feeds.Common.StaticData;

namespace Trakx.MarketData.Feeds.Common.Trackers
{
    public interface ITrackerFactory
    {
        ITracker FromTicker(string ticker);
    }


    public static class TrackerConstants
    {
        public const string LeverageDirection = nameof(LeverageDirection);
        public const string LeverageAmplitude = nameof(LeverageAmplitude);
        public const string SymbolGroup = nameof(SymbolGroup);
        public const string BasketSizeGroup = nameof(BasketSizeGroup);
        public const string Long = "L";
        public const string Inverse = "I";
    }

    public class TrackerFactory : ITrackerFactory
    {
        protected readonly static string tickerPattern = "^(?<" + TrackerConstants.LeverageDirection
                                                                + $">({TrackerConstants.Inverse}|{TrackerConstants.Long}))(?<"
                                                                + TrackerConstants.LeverageAmplitude + ">[\\d]{1})(?<"
                                                                + TrackerConstants.SymbolGroup + ">("
                                                                + string.Join("|", TrackerSymbols.AllSymbols) + ")+)(?<"
                                                                + TrackerConstants.BasketSizeGroup + ">[\\d]*)$";

        private static Regex _regex = new Regex(tickerPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <inheritdoc />
        public ITracker FromTicker(string ticker)
        {
            try
            {
                var match = _regex.Matches(ticker).Single();

                var sign = match.Groups[TrackerConstants.LeverageDirection].Value.ToUpperInvariant().Equals(TrackerConstants.Long) ? 1 : -1;
                var amplitude = int.Parse(match.Groups[TrackerConstants.LeverageAmplitude].Value);
                var leverage = sign * amplitude;
                var symbol = match.Groups[TrackerConstants.SymbolGroup].Value.ToUpperInvariant();
                var basketSizeString = match.Groups[TrackerConstants.BasketSizeGroup].Value;
                var basketSize = string.IsNullOrEmpty(basketSizeString)
                                     ? 1 : int.Parse(basketSizeString);

                return new Tracker(leverage, symbol, basketSize);
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"Unable to create tracker from ticker {ticker}", e);
            }
        }
    }
}