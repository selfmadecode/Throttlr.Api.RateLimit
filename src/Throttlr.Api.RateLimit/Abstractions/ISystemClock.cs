using System;
using System.Collections.Generic;
using System.Text;

namespace Throttlr.Api.RateLimit.Abstractions
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
