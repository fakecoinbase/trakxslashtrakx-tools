﻿namespace Trakx.Data.Common.Interfaces.Index
{
    /// <summary>
    /// Represents an ERC20 compatible token which can be used as a component in an index.
    /// </summary>
    public interface IComponentDefinition
    {
        /// <summary>
        /// Address of the smart contract defining the ERC20 contract of the component.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Common (longer) name for the component (ex: Bitcoin, Ethereum, Litecoin).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Symbol of the token associated with this component (ex: ETH, BTC, LTC).
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Number of decimals by which 1 unit of the ERC20 can be divided.
        /// </summary>
        ushort Decimals { get; }

        /// <summary>
        /// The Id of the token on the CoinGeckoApi
        /// </summary>
        string? CoinGeckoId { get; }
    }
}