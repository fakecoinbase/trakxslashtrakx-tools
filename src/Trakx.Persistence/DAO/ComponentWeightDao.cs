using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Persistence.DAO
{
    /// <inheritdoc />
    public class ComponentWeightDao : IComponentWeight
    {        
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // This constructor is for serialisation only
        #pragma warning disable CS8618
        public ComponentWeightDao() {}
        #pragma warning restore CS8618

        public ComponentWeightDao(IndiceDefinitionDao indiceDefinition, 
            ComponentDefinitionDao componentDefinition,
            decimal weight)
        {
            Id = $"{indiceDefinition.Symbol}|{componentDefinition.Symbol}";
            ComponentDefinitionDao = componentDefinition;
            IndiceDefinitionDao = indiceDefinition;
            Weight = weight;
        }

        /// <summary>
        /// Unique identifier generated and used as a primary key on the database object.
        /// </summary>
        [Key]
        public string Id { get; private set; }

        public ComponentDefinitionDao ComponentDefinitionDao { get; set; }

        public IndiceDefinitionDao IndiceDefinitionDao { get; set; }

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
