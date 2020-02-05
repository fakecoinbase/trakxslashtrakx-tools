using System.Diagnostics;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Common.Core
{
    /// <inheritdoc />
    public class ComponentDefinition : IComponentDefinition
    {
        public ComponentDefinition() { }

        public ComponentDefinition(string address, string name, string symbol, int decimals)
        {
            Address = address;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;

            Debug.Assert(this.IsValid());
        }

        #region Implementation of IComponentDefinition

        /// <inheritdoc />
        public string Address { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Symbol { get; set; }

        /// <inheritdoc />
        public int Decimals { get; set; }

        #endregion
    }
}