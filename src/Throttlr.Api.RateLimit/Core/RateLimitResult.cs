using System;

namespace RateLimit.Throttlr.Core
{
    public sealed class RateLimitResult
    {
        /// <summary>
        /// Represents the result of a rate limit check.
        /// </summary>
        private RateLimitResult(bool isAllowed, int? retryAfterSeconds = null)
        {
            IsAllowed = isAllowed;
            RetryAfterSeconds = retryAfterSeconds;
        }

        /// <summary>
        /// Gets a value indicating whether the request is allowed.
        /// </summary>
        public bool IsAllowed { get; }

        /// <summary>
        /// Gets the number of seconds the client should wait before retrying, if denied.
        /// </summary>
        public int? RetryAfterSeconds { get; }

        /// <summary>
        /// Creates a result indicating the request is allowed.
        /// </summary>
        public static RateLimitResult Allowed() => new RateLimitResult(true);

        /// <summary>
        /// Creates a result indicating the request is denied.
        /// </summary>
        public static RateLimitResult Denied(int retryAfterSeconds) =>
            new RateLimitResult(false, retryAfterSeconds);

        /// <summary>
        /// Gets a value indicating whether the request was limited.
        /// </summary>
        public bool IsLimited { get; }

        /// <summary>
        /// Gets the number of remaining requests in the current window or bucket.
        /// May be negative if the request was rejected.
        /// </summary>
        public int Remaining { get; }

        /// <summary>
        /// Gets the absolute time when the limit will reset (UTC).
        /// Null if the limiter does not provide reset information.
        /// </summary>
        public DateTimeOffset? Reset { get; }

        /// <summary>
        /// Gets the retry-after duration if limited.
        /// Null if not applicable.
        /// </summary>
        public TimeSpan? RetryAfter { get; }

        /// <summary>
        /// Creates a successful (not limited) result.
        /// </summary>
        public static RateLimitResult Success(int remaining, DateTimeOffset? reset = null) =>
            new RateLimitResult(false, remaining, reset, null);

        /// <summary>
        /// Creates a limited result.
        /// </summary>
        public static RateLimitResult Limited(int remaining, DateTimeOffset? reset, TimeSpan? retryAfter) =>
            new RateLimitResult(true, remaining, reset, retryAfter);

        private RateLimitResult(bool isLimited, int remaining, DateTimeOffset? reset, TimeSpan? retryAfter)
        {
            IsLimited = isLimited;
            Remaining = remaining;
            Reset = reset;
            RetryAfter = retryAfter;
        }
    }
}
