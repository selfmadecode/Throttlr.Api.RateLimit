namespace Throttlr.Api.RateLimit.Core
{
    internal sealed class RateLimitResult
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
    }
}
