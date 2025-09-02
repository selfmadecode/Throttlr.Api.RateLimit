using System;

namespace RateLimit.Throttlr.Abstractions
{
    /// <summary>
    /// Provides an abstraction for accessing the current UTC system clock.
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}
