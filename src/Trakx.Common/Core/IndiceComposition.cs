using System;
using System.Collections.Generic;
using System.Diagnostics;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Core
{
    /// <inheritdoc />
    public class IndiceComposition : IIndiceComposition
    {
        public IndiceComposition(IIndiceDefinition definition, List<IComponentQuantity> components, uint version,
            DateTime creationDate, string address)
        {
            IndiceDefinition = definition;
            ComponentQuantities = components;
            Version = version;
            CreationDate = creationDate;
            Address = address;
            Symbol = definition.GetCompositionSymbol(creationDate);

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IIndiceComposition

        /// <inheritdoc />
        public string Address { get; }

        /// <inheritdoc />
        public string Symbol { get; }

        /// <inheritdoc />
        public IIndiceDefinition IndiceDefinition { get; }

        /// <inheritdoc />
        public List<IComponentQuantity> ComponentQuantities { get; }

        /// <inheritdoc />
        public uint Version { get;  }

        /// <inheritdoc />
        public DateTime CreationDate { get; }

        #endregion
    }
}