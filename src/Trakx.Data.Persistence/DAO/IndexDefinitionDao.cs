using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Persistence.DAO
{
    public class IndexDefinitionDao : IIndexDefinition
    {
        public IndexDefinitionDao(string symbol, string name, string description,
            ushort naturalUnit, string address,
            DateTime? creationDate)
        {
            Symbol = symbol;
            Name = name;
            Description = description;
            ComponentWeightDaos = new List<ComponentWeightDao>();
            NaturalUnit = naturalUnit;
            Address = address;
            CreationDate = creationDate;
            IndexCompositionDaos = new List<IndexCompositionDao>();
        }

        public List<ComponentWeightDao> ComponentWeightDaos { get; set; }
        public List<IndexCompositionDao> IndexCompositionDaos { get; set; }

        /// <inheritdoc />
        [Key, Required, MaxLength(50)]
        public string Symbol { get; set; }

        /// <inheritdoc />
        [Required, MaxLength(512)]
        public string Name { get; set; }

        /// <inheritdoc />
        [Required]
        public string Description { get; set; }

        /// <inheritdoc />
        [NotMapped]
        public List<IComponentWeight> ComponentWeights => 
            ComponentWeightDaos.Cast<IComponentWeight>().ToList();

        /// <inheritdoc />
        [MaxLength(256)]
        public string Address { get; set; }

        /// <inheritdoc />
        [Required]
        public ushort NaturalUnit { get; set; }

        /// <inheritdoc />
        public DateTime? CreationDate { get; set; }
    }
}