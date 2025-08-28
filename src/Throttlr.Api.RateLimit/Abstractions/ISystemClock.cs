using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimit.Throttlr.Abstractions
{
    /// <summary>
    /// Provides an abstraction for accessing the current UTC system clock.
    /// Useful for unit testing and time-based rate limiting algorithms.
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}
