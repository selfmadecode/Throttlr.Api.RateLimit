using RateLimit.Throttlr.Core;

namespace RateLimit.Throttlr
{
    /// <summary>
    /// Represents a policy that determines how rate limits are applied
    /// to incoming requests.
    /// </summary>
    internal interface IRateLimitPolicy
    {
        /// <summary>
        /// Gets the unique identifier for this policy (e.g., "per-user", "per-ip").
        /// </summary>
        string PolicyName { get; }

        /// <summary>
        /// Generates a partition key for rate limiting based on the given context.
        /// For example, this could be a user ID, IP address, or API route.
        /// </summary>
        /// <param name="context">The current rate limit context.</param>
        /// <returns>A string key used to track rate limit counters.</returns>
        string GetPartitionKey(RateLimitContext context);
    }
}
