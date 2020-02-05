using System.ComponentModel.DataAnnotations;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Persistence.DAO
{
    public class ComponentDefinitionDao : IComponentDefinition
    {
        /// <inheritdoc />
        [Key, MaxLength(256)]
        public string Address { get; set; }

        /// <inheritdoc />
        [MaxLength(512)]
        public string Name { get; set; }

        /// <inheritdoc />
        [MaxLength(50)]
        public string Symbol { get; set; }

        /// <inheritdoc />
        public int Decimals { get; set; }
    }
}