using System;
using System.Collections.Generic;
using System.Diagnostics;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Common.Core
{
    /// <inheritdoc />
    public class IndexComposition : IIndexComposition
    {
        //public IndexComposition() { }

        public IndexComposition(IIndexDefinition definition, List<IComponentQuantity> components, uint version,
            DateTime creationDate)
        {
            IndexDefinition = definition;
            ComponentQuantities = components;
            Version = version;
            CreationDate = creationDate;

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IIndexComposition

        /// <inheritdoc />
        public IIndexDefinition IndexDefinition { get; }

        /// <inheritdoc />
        public List<IComponentQuantity> ComponentQuantities { get; }

        /// <inheritdoc />
        public uint Version { get;  }

        /// <inheritdoc />
        public DateTime CreationDate { get; }

        #endregion
    }
}