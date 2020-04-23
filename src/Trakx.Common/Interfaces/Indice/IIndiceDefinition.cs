using System;

namespace Trakx.Common.Interfaces.Indice
{
    /// <summary>
    /// Represents an indice with an immutable definition. Not to be confused with
    /// <see cref="IIndiceComposition"/> which is an instance of the indice where the
    /// quantity for each component is assigned as a result of component valuations
    /// and weight redistribution.
    /// </summary>
    public interface IIndiceDefinition
    {
        /// <summary>
        /// Symbol of the token associated with this indice (ex: L1MKC005).
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Name (long) given to the indice (ex: Top 5 Market Cap)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A brief explanation of the indice and the choice of components it contains.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Expressed as a power of 10, it represents the minimal amount of the token
        /// representing the indice that one can buy. 
        /// </summary>
        ushort NaturalUnit { get; }

        /// <summary>
        /// If the indice was created, the address at which the corresponding smart contract
        /// can be found on chain.
        /// </summary>
        string? Address { get; }

        /// <summary>
        /// Date at which the indice was created.
        /// </summary>
        DateTime? CreationDate { get; }
    }

    public static class IndiceDefinitionExtensions
    {
        public static string GetCompositionSymbol(this IIndiceDefinition indiceDefinition, DateTime issueDate)
                => $"{indiceDefinition.Symbol}{issueDate:yyMM}";
    }
}