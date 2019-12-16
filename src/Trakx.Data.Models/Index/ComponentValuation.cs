using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trakx.Data.Models.Index
{
    public class ComponentValuation
    {
        public ComponentValuation() { }

        public ComponentValuation(
            ComponentDefinition definition,
            string quoteCurrency,
            decimal price,
            DateTime? timeStamp = default)
        {
            Definition = definition;
            QuoteCurrency = quoteCurrency;
            Price = price;
            Value = price * (decimal)definition.Quantity * (decimal)Math.Pow(10, - definition.Decimals);
            TimeStamp = timeStamp ?? DateTime.UtcNow;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ComponentValuationId { get; set; }

        /// <summary>
        /// Date at which the valuation calculation was performed
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Component definition to which the valuation is linked.
        /// </summary>
        public ComponentDefinition Definition { get; }

        /// <summary>
        /// Currency in which the valuation is expressed.
        /// </summary>
        [Column(TypeName = "NVARCHAR(50)")]
        public string QuoteCurrency { get; set; }

        /// <summary>
        /// Price at which 1 unit of the token can currently be purchased.
        /// </summary>
        [Column(TypeName = "decimal(38, 18)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Current Total USD value of the component
        /// Basically <see cref="Price"/> times <see cref="ComponentDefinition.Quantity"/> adjusted to
        /// the correct number of decimals.
        /// </summary>
        [Column(TypeName = "decimal(38, 18)")]
        public decimal Value { get; set; }
    }
}