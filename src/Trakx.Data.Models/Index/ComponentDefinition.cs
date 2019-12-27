using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Trakx.Data.Models.Index
{
    public class ComponentDefinition : IComponentDefinition
    {
        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [JsonIgnore]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <inheritdoc />
        [MaxLength(256)]
        public string Address { get; set; }

        /// <inheritdoc />
        [MaxLength(512)]
        public string Name { get; set; }

        /// <inheritdoc />
        [MaxLength(50)]
        public string Symbol { get; set; }

        /// <inheritdoc />
        public int Decimals { get; set; }

        /// <inheritdoc />
        public ulong Quantity { get; set; }

        /// <summary>
        /// Url of an icon used to represent the component.
        /// </summary>
        [NotMapped]
        public string IconUrl { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        [ForeignKey(nameof(ComponentValuation.ComponentDefinition))]
        public ComponentValuation InitialValuation { get; set; }

        public ComponentDefinition() { }

        public ComponentDefinition(string address,
            string name,
            string symbol,
            int decimals,
            ulong quantity,
            decimal initialPrice,
            string quoteCurrency,
            DateTime valuationDateTime,
            int naturalUnit,
            string iconUrl = default)
        {
            Address = address;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Quantity = quantity;
            InitialValuation =
                new ComponentValuation(this, quoteCurrency, initialPrice, naturalUnit, valuationDateTime);
            IconUrl = iconUrl;
        }
    }
}