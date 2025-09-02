using System;
using System.Collections.Generic;
using System.Text;
using RateLimit.Throttlr.Abstractions;

namespace RateLimit.Throttlr.Core
{
    public sealed class SystemClock : ISystemClock
    {
        /// <inheritdoc />
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
