using System;
using System.Numerics;

namespace Trakx.Data.Models.Index
{
    public class ComponentValuation
    {
        public ComponentValuation() { }

        public ComponentValuation(string quoteCurrency, BigInteger quantity, int decimals, decimal price)
        {
            QuoteCurrency = quoteCurrency;
            Price = price;
            Value = price * (decimal)quantity * (decimal)Math.Pow(10, -decimals);
        }

        /// <summary>
        /// Date at which the valuation calculation was performed
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Currency in which the valuation is expressed.
        /// </summary>
        public string QuoteCurrency { get; set; }

        /// <summary>
        /// Price at which 1 unit of the token can currently be purchased.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Current Total USD value of the component
        /// Basically <see cref="Price"/> times <see cref="ComponentDefinition.Quantity"/> adjusted to
        /// the correct number of decimals.
        /// </summary>
        public decimal Value { get; set; }
    }
}