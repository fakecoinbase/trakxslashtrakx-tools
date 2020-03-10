using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Trakx.Data.Common.Interfaces.Index
{
    /// <summary>
    /// Represents an index with an immutable definition. Not to be confused with
    /// <see cref="IIndexComposition"/> which is an instance of the index where the
    /// quantity for each component is assigned as a result of component valuations
    /// and weight redistribution.
    /// </summary>
    public interface IIndexDefinition
    {
        /// <summary>
        /// Symbol of the token associated with this index (ex: L1MKC005).
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Name (long) given to the index (ex: Top 5 Market Cap)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A brief explanation of the index and the choice of components it contains.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Expressed as a power of 10, it represents the minimal amount of the token
        /// representing the index that one can buy. 
        /// </summary>
        ushort NaturalUnit { get; }

        /// <summary>
        /// If the index was created, the address at which the corresponding smart contract
        /// can be found on chain.
        /// </summary>
        string? Address { get; }

        /// <summary>
        /// Date at which the index was created.
        /// </summary>
        DateTime? CreationDate { get; }
    }

    public static class IndexDefinitionExtensions
    {
        public static string GetCompositionSymbol(this IIndexDefinition indexDefinition, DateTime issueDate)
                => $"{indexDefinition.Symbol}{issueDate:yyMM}";
    }
}