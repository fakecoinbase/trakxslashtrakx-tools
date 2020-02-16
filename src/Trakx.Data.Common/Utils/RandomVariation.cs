using System;

namespace Trakx.Data.Common.Utils
{
    public static class RandomVariation
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// A convenience method to add a random variation to a given value.
        /// </summary>
        /// <param name="original">The original value around which we want a variation.</param>
        /// <param name="maxPercentageVariation">The maximal amplitude of the variation.</param>
        /// <returns></returns>
        public static decimal AddRandomVariation(this decimal original, decimal maxPercentageVariation)
        {
            var variation = (decimal)(2  * (Random.NextDouble() - 0.5));
            var randomMove = original * maxPercentageVariation * variation;
            return original + randomMove;
        }
    }
}
