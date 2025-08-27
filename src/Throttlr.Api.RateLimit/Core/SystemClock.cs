using System;
using System.Collections.Generic;
using System.Text;
using RateLimit.Throttlr.Abstractions;

namespace RateLimit.Throttlr.Core
{
    /// <summary>
    /// Default implementation of <see cref="ISystemClock"/> that uses <see cref="DateTimeOffset.UtcNow"/>.
    /// </summary>
    public sealed class SystemClock : ISystemClock
    {
        /// <inheritdoc />
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
