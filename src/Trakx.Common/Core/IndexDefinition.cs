using System;
using System.Diagnostics;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Common.Core
{
    public class IndexDefinition : IIndexDefinition
    {
        public IndexDefinition(string symbol, string name, string description,
            ushort naturalUnit, string address, DateTime? creationDate)
        {
            Symbol = symbol;
            Name = name;
            Description = description;
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
        public ushort NaturalUnit { get; }

        /// <inheritdoc />
        public string Address { get; }

        /// <inheritdoc />
        public DateTime? CreationDate { get; }

        #endregion
    }
}