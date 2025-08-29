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
        /// Checks if the given key is allowed under the rate limit policy.
        /// </summary>
        Task<RateLimitResult> ShouldLimitAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the configured limit for this rate limiter.
        /// </summary>
        int GetLimit();
    }
}
