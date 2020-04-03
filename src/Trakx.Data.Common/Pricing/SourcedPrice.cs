using System;

namespace Trakx.Data.Common.Pricing
{
    public struct SourcedPrice :IEquatable<SourcedPrice>
    {
        public SourcedPrice(string id, string source, decimal price)
        {
            Id = id;
            Source = source;
            Price = price;
        }

        public string Id { get; }
        public string Source { get; }
        public decimal Price { get; }

        #region Equality members

        /// <inheritdoc />
        public bool Equals(SourcedPrice other)
        {
            return string.Equals(Id, other.Id, StringComparison.InvariantCulture)
                   && string.Equals(Source, other.Source, StringComparison.InvariantCultureIgnoreCase)
                   && Price == other.Price;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is SourcedPrice other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StringComparer.InvariantCulture.GetHashCode(Id);
                hashCode = (hashCode * 397) ^ StringComparer.InvariantCultureIgnoreCase.GetHashCode(Source);
                hashCode = (hashCode * 397) ^ Price.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(SourcedPrice left, SourcedPrice right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SourcedPrice left, SourcedPrice right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}