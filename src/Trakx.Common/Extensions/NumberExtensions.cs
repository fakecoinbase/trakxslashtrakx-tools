using System;
using System.Numerics;

namespace Trakx.Common.Extensions
{
    public static class NumberExtensions
    {
        public static BigInteger AsAPowerOf10(this ushort number) => BigInteger.Pow(10, number);
        public static decimal AsAPowerOf10(this int number) => (decimal)Math.Pow(10, number);

        /// <summary>
        /// Takes a quantity as expressed in set protocol, in the number of units native to the
        /// components and scales it as a number of units native to the index to which it belongs.
        /// </summary>
        /// <param name="unscaledQuantity">Quantity in number units native to the component.</param>
        /// <param name="componentDecimals">Number of decimals supported by by the ERC20 contract of the component.</param>
        /// <param name="indexNaturalUnit">Natural unit of the index, see set protocol for more details. Generally 10
        /// as it is the convention for rebalancing indices.</param>
        /// <returns>Quantity in number units native to the index.</returns>
        /// <remarks>18 is the number of decimals of the contracts produced by the set factory.</remarks>
        public static decimal ScaleComponentQuantity(this decimal unscaledQuantity, ushort componentDecimals,
            ushort indexNaturalUnit) => unscaledQuantity * (decimal)(18 - componentDecimals - indexNaturalUnit).AsAPowerOf10();

        /// <summary>
        /// Takes a quantity expressed as a number of units native to the index to which it belongs, and returns it
        /// as expressed in set protocol, in the number of units native to the components and scales it.
        /// </summary>
        /// <param name="scaledQuantity">Quantity in number units native to the index.</param>
        /// <param name="componentDecimals">Number of decimals supported by by the ERC20 contract of the component.</param>
        /// <param name="indexNaturalUnit">Natural unit of the index, see set protocol for more details. Generally 10
        /// as it is the convention for rebalancing indices.</param>
        /// <returns>Quantity in number units native to the component.</returns>
        /// <remarks>18 is the number of decimals of the contracts produced by the set factory.</remarks>
        public static decimal DescaleComponentQuantity(this decimal scaledQuantity, ushort componentDecimals,
            ushort indexNaturalUnit) => scaledQuantity * (decimal)(componentDecimals + indexNaturalUnit - 18).AsAPowerOf10();
    }
}
