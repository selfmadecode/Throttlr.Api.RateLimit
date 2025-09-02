using System;

namespace RateLimit.Throttlr.Core.Counter
{
    /// <summary>
    /// Represents a counter for sliding window rate limiting.
    /// Tracks requests in both the current and previous window segment.
    /// </summary>
    public sealed class SlidingWindowCounter
    {
        /// <summary>
        /// Number of requests in the current window.
        /// </summary>
        public int RequestsInCurrentWindow { get; }

        /// <summary>
        /// Number of requests in the previous window.
        /// </summary>
        public int RequestsInPreviousWindow { get; }

        /// <summary>
        /// Start timestamp of the current window.
        /// </summary>
        public DateTimeOffset WindowStart { get; }

        /// <summary>
        /// Expiration time for this window counter.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlidingWindowCounter"/> class.
        /// </summary>
        public SlidingWindowCounter(
            int requestsInCurrent,
            DateTimeOffset windowStart,
            DateTimeOffset expiresAt,
            int requestsInPrevious = 0)
        {
            RequestsInCurrentWindow = requestsInCurrent;
            RequestsInPreviousWindow = requestsInPrevious;
            WindowStart = windowStart;
            ExpiresAt = expiresAt;
        }

        /// <summary>
        /// Increments the request count, rolling over if the window has passed.
        /// </summary>
        public SlidingWindowCounter Increment(DateTimeOffset now, RateLimitPolicy policy)
        {
            var windowEnd = WindowStart.Add(policy.Window);

            if (now >= windowEnd)
            {
                // Roll into new window
                return new SlidingWindowCounter(
                    requestsInCurrent: 1,
                    windowStart: now,
                    expiresAt: now.Add(policy.Window),
                    requestsInPrevious: RequestsInCurrentWindow);
            }

            // Still in current window
            return new SlidingWindowCounter(
                requestsInCurrent: RequestsInCurrentWindow + 1,
                windowStart: WindowStart,
                expiresAt: ExpiresAt,
                requestsInPrevious: RequestsInPreviousWindow);
        }
    }
}
