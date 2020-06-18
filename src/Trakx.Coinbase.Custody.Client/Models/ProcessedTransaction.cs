using System;

namespace Trakx.Coinbase.Custody.Client.Models
{
    public class ProcessedTransaction : Transaction
    {
        private readonly long _chainDecimal;
       

        public decimal DecimalAmount { get; }

        public ProcessedTransaction(long chainDecimal,Transaction transaction):base(transaction)
        {
            _chainDecimal = chainDecimal;
            DecimalAmount=Amount * -(decimal)Math.Pow(10, chainDecimal);
        }
    }
}
