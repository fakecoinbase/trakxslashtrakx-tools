using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Persistence.DAO
{
    /// <inheritdoc />
    public class ComponentQuantityDao : IComponentQuantity
    {
        public ComponentQuantityDao() { }

        public ComponentQuantityDao(IndexCompositionDao indexComposition,
            ComponentDefinitionDao componentDefinition, 
            ulong unscaledQuantity)
        {
            IndexCompositionDao = indexComposition;
            ComponentDefinitionDao = componentDefinition;
            Quantity = unscaledQuantity * (decimal)Math.Pow(10, 18 - componentDefinition.Decimals - indexComposition.IndexDefinitionDao.NaturalUnit);
            UpdateId();
        }

        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [Key]
        public string Id { get; set; }

        public ComponentDefinitionDao ComponentDefinitionDao { get; set; }

        public IndexCompositionDao IndexCompositionDao { get; set; }

        #region Implementation of IComponentWeight

        /// <inheritdoc />
        [NotMapped]
        public IComponentDefinition ComponentDefinition => ComponentDefinitionDao;

        /// <inheritdoc />
        [Required, Column(TypeName = "decimal(38, 18)")]
        public decimal Quantity { get; set; }

        #endregion

        public void LinkToIndexComposition(IndexCompositionDao indexComposition)
        {
            IndexCompositionDao = indexComposition;
            UpdateId();
        }

        private void UpdateId()
        {
            Id = $"{IndexCompositionDao.Id}|{ComponentDefinition.Symbol}";
        }
    }
}
