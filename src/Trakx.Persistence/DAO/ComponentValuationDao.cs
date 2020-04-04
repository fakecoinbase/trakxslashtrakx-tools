using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Persistence.DAO
{
    /// <inheritdoc />
    public class ComponentValuationDao : IComponentValuation
    {
        public ComponentValuationDao() {}

        public ComponentValuationDao(ComponentQuantityDao componentComponentQuantity, DateTime timeStamp, string quoteCurrency,
            decimal price, string priceSource)
        {
            Id = $"{componentComponentQuantity.Id}|{quoteCurrency}|{timeStamp:yyMMddHHmmssff}";
            ComponentQuantityDao = componentComponentQuantity;
            QuoteCurrency = quoteCurrency;
            TimeStamp = timeStamp;
            Price = price;
            PriceSource = priceSource;
            Value = price * ComponentQuantityDao.Quantity;
        }

        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [Key]
        public string Id { get; private set; }

        /// <inheritdoc />
        [Column(TypeName = "decimal(10, 10)")]
        public double? Weight { get; set; }

        /// <inheritdoc />
        public DateTime TimeStamp { get; set; }

        /// <inheritdoc />
        public void SetWeightFromTotalValue(decimal totalIndexValue)
        {
            Weight = (double)this.GetWeightFromTotalValue(totalIndexValue);
        }

        /// <inheritdoc />
        [NotMapped]
        public IComponentQuantity ComponentQuantity => ComponentQuantityDao;

        public ComponentQuantityDao ComponentQuantityDao { get; set; }

        /// <inheritdoc />
        [MaxLength(50)]
        public string QuoteCurrency { get; set; }

        /// <inheritdoc />
        [Column(TypeName = "decimal(38, 18)")]
        public decimal Price { get; set; }

        /// <inheritdoc />
        [MaxLength(50)]
        public string PriceSource { get; set; }

        /// <inheritdoc />
        [Column(TypeName = "decimal(38, 18)")]
        public decimal Value { get; set; }
    }
}