using System;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class CoinbaseTransaction : CoinbaseRawTransaction
    {
        /// <summary>
        /// Amount expressed as a fraction of the native unit of the token
        /// being transacted. For instance, in an ethereum transaction, the
        /// <see cref="CoinbaseRawTransaction.UnscaledAmount"/> is in Wei, while the
        /// <see cref="ScaledAmount"/> is in Eth.
        /// </summary>
        public decimal ScaledAmount { get; }

        public CoinbaseTransaction(CoinbaseRawTransaction coinbaseRawTransaction, ushort chainDecimal) 
            : base(coinbaseRawTransaction)
        {
            ScaledAmount = UnscaledAmount * (decimal)Math.Pow(10, -chainDecimal);
        }
    }
}
