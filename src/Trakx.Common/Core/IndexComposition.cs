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
            DateTime creationDate, string address)
        {
            IndexDefinition = definition;
            ComponentQuantities = components;
            Version = version;
            CreationDate = creationDate;
            Address = address;
            Symbol = $"{definition.Symbol}{creationDate:yyMM}";

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IIndexComposition

        /// <inheritdoc />
        public string Address { get; }

        /// <inheritdoc />
        public string Symbol { get; }

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