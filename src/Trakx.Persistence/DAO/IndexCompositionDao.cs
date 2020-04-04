using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Persistence.DAO
{
    public class IndexCompositionDao : IIndexComposition
    {
        public IndexCompositionDao(string address, string symbol)
        {
            Address = address;
            Symbol = symbol;
        }

        public IndexCompositionDao(IndexDefinitionDao indexDefinition, 
            uint version, DateTime creationDate, string address, string symbol)
        {
            Id = $"{indexDefinition.Symbol}|{version}";
            IndexDefinitionDao = indexDefinition;
            Version = version;
            CreationDate = creationDate;
            Address = address;
            Symbol = $"{indexDefinition.Symbol}{creationDate:yyMM}";
            ComponentQuantityDaos = new List<ComponentQuantityDao>();
            IndexValuationDaos = new List<IndexValuationDao>();
        }

        public List<ComponentQuantityDao> ComponentQuantityDaos { get; set; }

        #region Implementation of IIndexComposition
        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [Key]
        public string Id { get; private set; }

        /// <inheritdoc />
        public string Address { get; set; }

        /// <inheritdoc />
        public string Symbol { get; set; }

        /// <inheritdoc />
        [NotMapped]
        public IIndexDefinition IndexDefinition => IndexDefinitionDao;

        public IndexDefinitionDao IndexDefinitionDao { get; set; }

        public List<IndexValuationDao> IndexValuationDaos { get; set; }

        /// <inheritdoc />
        [NotMapped]
        public List<IComponentQuantity> ComponentQuantities => 
            ComponentQuantityDaos.Cast<IComponentQuantity>().ToList();

        /// <inheritdoc />
        public uint Version { get; set; }

        /// <inheritdoc />
        public DateTime CreationDate { get; set; }

        #endregion
    }
}
