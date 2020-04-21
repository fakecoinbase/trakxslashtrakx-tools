using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Persistence.DAO
{
    /// <inheritdoc />
    public class ComponentQuantityDao : IComponentQuantity
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // This constructor is for serialisation only
        #pragma warning disable CS8618
        public ComponentQuantityDao() { }

        public ComponentQuantityDao(IndexCompositionDao indexComposition,
            ComponentDefinitionDao componentDefinition, 
            ulong unscaledQuantity)
        {
            IndexCompositionDao = indexComposition;
            ComponentDefinitionDao = componentDefinition;
            Quantity = ((decimal)unscaledQuantity).ScaleComponentQuantity(componentDefinition.Decimals, indexComposition.IndexDefinitionDao.NaturalUnit);
            UpdateId();
        }
        #pragma warning restore CS8618

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
