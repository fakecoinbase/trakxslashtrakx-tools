using System.Diagnostics;
using Ardalis.GuardClauses;
using Trakx.Common.Extensions;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Core
{
    /// <inheritdoc />
    public class ComponentQuantity : IComponentQuantity
    {
        /// <param name="definition">Definition of the component</param>
        /// <param name="unscaledQuantity">Unscaled quantity of the component, as returned by Set protocol</param>
        /// <param name="indiceNaturalUnit">Scaling factor, expressed as a power of 10:
        /// 18 - <see cref="IComponentDefinition.Decimals"/> - <see cref="IIndiceDefinition.NaturalUnit"/></param>
        public ComponentQuantity(IComponentDefinition definition, ulong unscaledQuantity, ushort indiceNaturalUnit)
            : this(definition, (decimal) unscaledQuantity, indiceNaturalUnit) { }

        /// <param name="definition">Definition of the component</param>
        /// <param name="unscaledQuantity">Unscaled quantity of the component, as returned by Set protocol</param>
        /// <param name="indiceNaturalUnit">Scaling factor, expressed as a power of 10:
        /// 18 - <see cref="IComponentDefinition.Decimals"/> - <see cref="IIndiceDefinition.NaturalUnit"/></param>
        public ComponentQuantity(IComponentDefinition definition, decimal unscaledQuantity, ushort indiceNaturalUnit)
        {
            Guard.Against.OutOfRange(indiceNaturalUnit, nameof(indiceNaturalUnit),
                0, 18);

            ComponentDefinition = definition;
            Quantity = unscaledQuantity.ScaleComponentQuantity(ComponentDefinition.Decimals, indiceNaturalUnit);

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
