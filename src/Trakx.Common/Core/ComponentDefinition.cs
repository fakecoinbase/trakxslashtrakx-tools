using System.Diagnostics;
using Trakx.Common.Interfaces.Indice;

namespace Trakx.Common.Core
{
    /// <inheritdoc />
    public class ComponentDefinition : IComponentDefinition
    {
        public ComponentDefinition() { }

        public ComponentDefinition(string address, string name, string symbol, string coinGeckoId, ushort decimals)
        {
            Address = address;
            Name = name;
            Symbol = symbol;
            CoinGeckoId = coinGeckoId;
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
        public ushort Decimals { get; set; }

        /// <inheritdoc />
        public string CoinGeckoId { get; }

        #endregion
    }
}