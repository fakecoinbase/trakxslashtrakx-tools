using System;
using System.Collections.Generic;
using System.Diagnostics;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Common.Core
{
    public class IndexDefinition : IIndexDefinition
    {
        public IndexDefinition(string symbol, string name, string description,
            List<IComponentWeight> componentWeights, ushort naturalUnit, string address,
            DateTime? creationDate)
        {
            Symbol = symbol;
            Name = name;
            Description = description;
            ComponentWeights = componentWeights;
            NaturalUnit = naturalUnit;
            Address = address;
            CreationDate = creationDate;

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IIndexDefinition

        /// <inheritdoc />
        public string Symbol { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public List<IComponentWeight> ComponentWeights { get; }

        /// <inheritdoc />
        public ushort NaturalUnit { get; }

        /// <inheritdoc />
        public string Address { get; }

        /// <inheritdoc />
        public DateTime? CreationDate { get; }

        #endregion
    }
}