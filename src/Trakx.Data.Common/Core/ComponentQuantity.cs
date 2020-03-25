using System.Diagnostics;
using Ardalis.GuardClauses;
using Trakx.Data.Common.Extensions;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Common.Core
{
    /// <inheritdoc />
    public class ComponentQuantity : IComponentQuantity
    {
        /// <param name="definition">Definition of the component</param>
        /// <param name="unscaledQuantity">Unscaled quantity of the component, as returned by Set protocol</param>
        /// <param name="indexNaturalUnit">Scaling factor, expressed as a power of 10:
        /// 18 - <see cref="IComponentDefinition.Decimals"/> - <see cref="IIndexDefinition.NaturalUnit"/></param>
        public ComponentQuantity(IComponentDefinition definition, ulong unscaledQuantity, ushort indexNaturalUnit)
            : this(definition, (decimal) unscaledQuantity, indexNaturalUnit) { }

        /// <param name="definition">Definition of the component</param>
        /// <param name="unscaledQuantity">Unscaled quantity of the component, as returned by Set protocol</param>
        /// <param name="indexNaturalUnit">Scaling factor, expressed as a power of 10:
        /// 18 - <see cref="IComponentDefinition.Decimals"/> - <see cref="IIndexDefinition.NaturalUnit"/></param>
        public ComponentQuantity(IComponentDefinition definition, decimal unscaledQuantity, ushort indexNaturalUnit)
        {
            Guard.Against.OutOfRange(indexNaturalUnit, nameof(indexNaturalUnit),
                0, 18);

            ComponentDefinition = definition;
            Quantity = unscaledQuantity.ScaleComponentQuantity(ComponentDefinition.Decimals, indexNaturalUnit);

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IComponent

        /// <inheritdoc />
        public IComponentDefinition ComponentDefinition { get; }

        /// <inheritdoc />
        public decimal Quantity { get; }
        
        #endregion
    }
}
