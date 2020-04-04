using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Trakx.Data.Common.Interfaces.Index;

namespace Trakx.Data.Persistence.DAO
{
    public class ComponentDefinitionDao : IComponentDefinition
    {
        public ComponentDefinitionDao() {}

        public ComponentDefinitionDao(string address, string name, string symbol, string coinGeckoId, ushort decimals)
        {
            Address = address;
            Name = name;
            Symbol = symbol;
            CoinGeckoId = coinGeckoId;
            Decimals = decimals;

            Debug.Assert(this.IsValid());
        }

        /// <inheritdoc />
        [Key, MaxLength(256)]
        public string Address { get; set; }

        /// <inheritdoc />
        [MaxLength(512)]
        public string Name { get; set; }

        /// <inheritdoc />
        [MaxLength(50)]
        public string Symbol { get; set; }

        /// <inheritdoc />
        public ushort Decimals { get; set; }

        /// <inheritdoc />
        [MaxLength(256)]
        public string CoinGeckoId { get; set; }
    }
}