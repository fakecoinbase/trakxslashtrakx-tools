﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Persistence.DAO
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
            NaturalUnit = naturalUnit;
            Address = address;
            CreationDate = creationDate;
            IndexCompositionDaos = new List<IndexCompositionDao>();
        }

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
        [MaxLength(256)]
        public string Address { get; set; }

        /// <inheritdoc />
        [Required]
        public ushort NaturalUnit { get; set; }

        /// <inheritdoc />
        public DateTime? CreationDate { get; set; }
    }
}