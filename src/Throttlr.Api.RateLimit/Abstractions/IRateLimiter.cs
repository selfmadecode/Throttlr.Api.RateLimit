using System.Threading;
using System.Threading.Tasks;
using RateLimit.Throttlr.Core;

namespace RateLimit.Throttlr
{
    /// <summary>
    /// Defines the contract for a rate limiter implementation.
    /// </summary>
    internal interface IRateLimiter
    {
        /// <summary>
        /// Attempts to acquire permission for a request based on the provided context.
        /// </summary>
        /// <param name="context">The rate limiting context (e.g., request key, metadata).</param>
        /// <param name="cancellationToken">A token to observe while waiting for permission.</param>
        /// <returns>A <see cref="RateLimitResult"/> representing the outcome of the request.</returns>
        //Task<RateLimitResult> AcquireAsync(RateLimitContext context,
        //    CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the given key is allowed under the rate limit policy.
        /// </summary>
        Task<RateLimitResult> ShouldLimitAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the configured limit for this rate limiter.
        /// </summary>
        int GetLimit();
    }
}
