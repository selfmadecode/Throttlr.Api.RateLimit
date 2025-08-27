using System;

namespace Throttlr.Api.RateLimit.Core.Counter
{
    /// <summary>
    /// Represents the state of a rate limit counter.
    /// </summary>
    /// <summary>
    /// Represents a single rate limit counter entry in the store.
    /// Immutable for thread-safety.
    /// </summary>
    public sealed class RateLimitCounter
    {
        public int Count { get; }
        public DateTimeOffset ResetTime { get; }

        public RateLimitCounter(int count, DateTimeOffset resetTime)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            Count = count;
            ResetTime = resetTime;
        }
    }
}
