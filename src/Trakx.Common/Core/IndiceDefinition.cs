using System;
using System.Diagnostics;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Core
{
    public class IndiceDefinition : IIndiceDefinition
    {
        public IndiceDefinition(string symbol, string name, string description,
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

        #region Implementation of IIndiceDefinition

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