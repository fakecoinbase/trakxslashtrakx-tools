using Trakx.Data.Common.Sources.CryptoCompare.DTOs.Inbound;

namespace Trakx.Data.Common.Sources.CryptoCompare.DTOs.Outbound
{
    public abstract class ExchangeBaseQuoteSubscription : VolumeSubscription
    {
        protected ExchangeBaseQuoteSubscription(string type, string exchange, string baseCurrency, string quoteCurrency) 
            : base(type, baseCurrency)
        {
            Exchange = exchange;
            QuoteCurrency = quoteCurrency.ToUpper();
        }

        /// <summary>
        /// The exchange for which we want the data. This can
        /// also be CCCAGG for CryptoCompare's own Aggregate Index.
        /// </summary>
        public string Exchange { get; }

        /// <summary>
        /// Ticker of the currency in which we want the data to be quoted.
        /// </summary>
        public string QuoteCurrency { get; }

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Type}~{Exchange}~{BaseCurrency}~{QuoteCurrency}";
        }

        #endregion
    }

    public class AggregateIndexSubscription : ExchangeBaseQuoteSubscription
    {
        /// <inheritdoc />
        public AggregateIndexSubscription(string baseCurrency, string quoteCurrency)
            : base(AggregateIndex.TypeValue, "CCCAGG", baseCurrency, quoteCurrency) { }
    }

    public class TradeSubscription : ExchangeBaseQuoteSubscription
    {
        public const string TypeValue = AggregateIndex.TypeValue;

        /// <inheritdoc />
        public TradeSubscription(string exchange, string baseCurrency, string quoteCurrency)
            : base(Trade.TypeValue, exchange, baseCurrency, quoteCurrency) { }
    }

    public class TickerSubscription : ExchangeBaseQuoteSubscription
    {
        public const string TypeValue = AggregateIndex.TypeValue;

        /// <inheritdoc />
        public TickerSubscription(string exchange, string baseCurrency, string quoteCurrency)
            : base(Ticker.TypeValue, exchange, baseCurrency, quoteCurrency) { }
    }

    public class OhlcSubscription : ExchangeBaseQuoteSubscription
    {
        public const string TypeValue = AggregateIndex.TypeValue;

        /// <inheritdoc />
        public OhlcSubscription(string exchange, string baseCurrency, string quoteCurrency)
            : base("24", exchange, baseCurrency, quoteCurrency) { }
    }
}