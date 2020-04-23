using System;
using Ardalis.GuardClauses;

namespace Trakx.Common.Interfaces.Indice
{
    public interface IComponentValuation
    {
        /// <summary>
        /// Definition of the component being valued as part of an indice.
        /// </summary>
        IComponentQuantity ComponentQuantity { get; }

        /// <summary>
        /// Currency in which the valuation is expressed.
        /// </summary>
        string QuoteCurrency { get; }

        /// <summary>
        /// Price of one unit of the <see cref="ComponentQuantity"/>.
        /// </summary>
        decimal Price { get; }

        /// <summary>
        /// DataSource from which the price was taken
        /// </summary>
        string PriceSource { get; }

        /// <summary>
        /// Total value contributed to the indice NAV by the component.
        /// This is basically <see cref="Price"/> * <see cref="IComponentQuantity.Quantity"/>
        /// adjusted to common natural unit.
        /// </summary>
        decimal Value { get; }

        /// <summary>
        /// The weight of the component relative to the other bundled in the same indice,
        /// expressed as a percentage of the total value of the indice in the <see cref="QuoteCurrency"/>.
        /// </summary>
        double? Weight { get; }

        /// <summary>
        /// Date at which the valuation calculation was performed.
        /// </summary>
        DateTime TimeStamp { get; }

        /// <summary>
        /// Use this method to set the <see cref="Weight"/> of the component once you know the total
        /// value of the indice to which it belongs.
        /// </summary>
        /// <param name="totalIndiceValue">Value of the whole indice used to calculate the component's relative weight.</param>
        void SetWeightFromTotalValue(decimal totalIndiceValue);
    }


    public static class ComponentValuationExtensions
    {
        /// <summary>
        /// Use this method to set the <see cref="IComponentValuation.Weight"/> of the component once you know the total
        /// value of the indice to which it belongs.
        /// </summary>
        /// <param name="componentValuation">Component valuation for which the weight is getting calculated.</param>
        /// <param name="totalIndiceValue">Value of the whole indice used to calculate the component's relative weight.</param>
        /// <returns></returns>
        public static decimal GetWeightFromTotalValue(this IComponentValuation componentValuation, decimal totalIndiceValue)
        {
            Guard.Against.NegativeOrZero(totalIndiceValue, nameof(totalIndiceValue));
            var weight = componentValuation.Value / totalIndiceValue;
            return weight;
        }
    }
}