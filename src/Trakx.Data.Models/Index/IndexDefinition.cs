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
        public string Name { get; set; }

        /// <summary>
        /// A brief explanation of the index and the choice of components it contains.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of the components contained in the index.
        /// </summary>
        public List<T> ComponentDefinitions { get; set; }

        /// <summary>
        /// Net Asset Value of the index at creation time.
        /// </summary>
        public IndexValuation InitialValuation { get; set; }

        /// <summary>
        /// Expressed as a power of 10, it represents the minimal amount of the token
        /// representing the index that one can buy. 
        /// </summary>
        public int NaturalUnit { get; set; }

        /// <summary>
        /// Date at which the index was created.
        /// </summary>
        public DateTime CreationDate { get; set; }
    }

    public class IndexDefinition : AbstractIndex<ComponentDefinition>
    {
        public IndexDefinition() { }

        public IndexDefinition(List<ComponentDefinition> componentDefinitions)
        {
            ComponentDefinitions = componentDefinitions;
            NaturalUnit = 18 - componentDefinitions.Min(c => c.Decimals);
        }

        public static readonly IndexDefinition Default = new IndexDefinition();
    }
}