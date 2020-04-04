﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Trakx.Data.Common.Interfaces.Index
{
    /// <summary>
    /// Represents the composition of an index, which results from its definition (where the weighing per
    /// component in a given currency is defined), at a given point in time. Typically, at every re-balancing,
    /// a new composition is calculated from the original definition.
    /// </summary>
    public interface IIndexComposition
    {
        /// <summary>
        /// Address at which the initial composition of the index has been deployed on chain.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Symbol for the token under which the composition is traded.
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Definition of the index, from which the composition derives.
        /// </summary>
        IIndexDefinition IndexDefinition { get; }

        /// <summary>
        /// List of components with the quantities defining this composition.
        /// </summary>
        List<IComponentQuantity> ComponentQuantities { get; }

        /// <summary>
        /// Each re-balancing gives a new composition of the index as defined in <see cref="IndexDefinition"/>,
        /// this version should be increased every time a new composition is issued for a given definition.
        /// </summary>
        uint Version { get; }

        /// <summary>
        /// Date at which the composition was created.
        /// </summary>
        DateTime CreationDate { get; }
    }

    public static class IndexCompositionExtensions
    {
        public static List<string> GetComponentSymbols(this IIndexComposition composition)
        {
            return composition.ComponentQuantities.Select(s => s.ComponentDefinition.Symbol).ToList();
        }

        public static List<string> GetComponentCoinGeckoIds(this IIndexComposition composition)
        {
            return composition.ComponentQuantities.Select(s => s.ComponentDefinition.CoinGeckoId).ToList();
        }
    }
}