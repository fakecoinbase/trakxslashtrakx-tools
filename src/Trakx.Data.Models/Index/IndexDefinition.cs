using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace Trakx.Data.Models.Index
{
    public class IndexDefinition : IIndexDefinition<ComponentDefinition>
    {
        public IndexDefinition() { }

        public IndexDefinition(string symbol,
            string name,
            string description,
            List<ComponentDefinition> componentDefinitions,
            string address,
            int? naturalUnit = default,
            DateTime creationDate = default)
        {
            Symbol = symbol;
            Name = name;
            Description = description;
            ComponentDefinitions = componentDefinitions;
            //Natural unit should be calculated based on a target price and a precision
            NaturalUnit = naturalUnit ?? 18 - componentDefinitions.Min(c => c.Decimals);
            Address = address;
            CreationDate = creationDate;
            InitialValuation = new IndexValuation(componentDefinitions);
        }

        public static readonly IndexDefinition Default = new IndexDefinition();

        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [JsonIgnore]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <inheritdoc />
        [MaxLength(50)]
        public string Symbol { get; set; }

        /// <inheritdoc />
        [Required]
        [MaxLength(512)]
        public string Name { get; set; }

        /// <inheritdoc />
        [Required]
        public string Description { get; set; }

        /// <inheritdoc />
        [MaxLength(256)]
        public string Address { get; set; }

        /// <inheritdoc />
        [Required, NotNull]
        public List<ComponentDefinition> ComponentDefinitions { get; set; }

        /// <inheritdoc />
        [Required, NotNull]
        public IndexValuation InitialValuation { get; set; }

        /// <inheritdoc />
        public int NaturalUnit { get; set; }

        /// <inheritdoc />
        public DateTime? CreationDate { get; set; }
    }
}