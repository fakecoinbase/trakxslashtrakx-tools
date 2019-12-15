using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Trakx.Data.Models.Index
{
    public abstract class AbstractIndex<T> where T : ComponentDefinition
    {
        /// <summary>
        /// Symbol of the token associated with this index (ex: L1MKC005).
        /// </summary>
        [Key]
        public string Symbol { get; set; }

        /// <summary>
        /// Name (long) given to the index (ex: Top 5 Market Cap)
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// A brief explanation of the index and the choice of components it contains.
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// If the index was created, the address at which the corresponding smart contract
        /// can be found on chain.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// List of the components contained in the index.
        /// </summary>
        [Required]
        public List<T> ComponentDefinitions { get; set; }

        /// <summary>
        /// Net Asset Value of the index at creation time.
        /// </summary>
        [Required]
        public IndexValuation InitialValuation { get; set; }

        /// <summary>
        /// Expressed as a power of 10, it represents the minimal amount of the token
        /// representing the index that one can buy. 
        /// </summary>
        public int NaturalUnit { get; set; }

        /// <summary>
        /// Date at which the index was created.
        /// </summary>
        [Required]
        public DateTime? CreationDate { get; set; }
    }

    public class IndexDefinition : AbstractIndex<ComponentDefinition>
    {
        public IndexDefinition() { }

        public IndexDefinition(string symbol, 
            string name, 
            string description,
            List<ComponentDefinition> componentDefinitions,
            string address,
            DateTime creationDate = default)
        {
            Symbol = symbol;
            Name = name;
            Description = description;
            ComponentDefinitions = componentDefinitions;
            NaturalUnit = 18 - componentDefinitions.Min(c => c.Decimals);
            Address = address;
            CreationDate = creationDate;
        }

        public static readonly IndexDefinition Default = new IndexDefinition();
    }
}