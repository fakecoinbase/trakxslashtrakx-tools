using System.Diagnostics;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Core
{
    public class ComponentWeight : IComponentWeight
    {

        public ComponentWeight(IComponentDefinition definition, decimal weight)
        {
            ComponentDefinition = definition;
            Weight = weight;

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IComponentWeight

        /// <inheritdoc />
        public IComponentDefinition ComponentDefinition { get; }

        /// <inheritdoc />
        public decimal Weight { get; }

        #endregion
    }
}
