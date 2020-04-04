using System;

namespace Trakx.Common.Interfaces
{
    /// <summary>
    /// Allows easier testing, by setting fixed return values.
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        #region Implementation of IDateTimeProvider

        /// <inheritdoc />
        public DateTime UtcNow => DateTime.UtcNow;

        #endregion
    }
}