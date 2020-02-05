using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Persistence.DAO
{
    /// <inheritdoc />
    public class ComponentWeightDao : IComponentWeight
    {
        public ComponentWeightDao() {}

        public ComponentWeightDao(IndexDefinitionDao indexDefinition, 
            ComponentDefinitionDao componentDefinition,
            decimal weight)
        {
            Id = $"{indexDefinition.Symbol}|{componentDefinition.Symbol}";
            ComponentDefinitionDao = componentDefinition;
            IndexDefinitionDao = indexDefinition;
            Weight = weight;
        }

        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [Key]
        public string Id { get; private set; }

        public ComponentDefinitionDao ComponentDefinitionDao { get; set; }

        public IndexDefinitionDao IndexDefinitionDao { get; set; }

        #region Implementation of IComponentWeight

        /// <inheritdoc />
        [NotMapped]
        public IComponentDefinition ComponentDefinition => ComponentDefinitionDao;

        /// <inheritdoc />
        [Required, Column(TypeName = "decimal(10, 10)")]
        public decimal Weight { get; set; }

        #endregion
    }
}
