﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Trakx.Common.Interfaces.Index;

namespace Trakx.Persistence.DAO
{
    public class ComponentDefinitionDao : IComponentDefinition
    {
        // Non-nullable field is uninitialized. Consider declaring as nullable.
        // This constructor is for serialisation only
        #pragma warning disable CS8618
        public ComponentDefinitionDao() {}
        #pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

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