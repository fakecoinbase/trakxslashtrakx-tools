using System;

namespace Trakx.Common.Pricing
{
    public class NavUpdate
    {
        public NavUpdate() { }
        public NavUpdate(string symbol, decimal value)
        {
            Symbol = symbol;
            Value = value;
            TimeStamp = DateTimeOffset.UtcNow;
        }

        public DateTimeOffset TimeStamp { get; set; }
        public string Symbol { get; set; }
        public decimal Value { get; set; }
    }
}