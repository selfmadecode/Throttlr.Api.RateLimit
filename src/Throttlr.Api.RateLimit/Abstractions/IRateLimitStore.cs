using System.Threading;
using System.Threading.Tasks;
using Throttlr.Api.RateLimit.Core;

namespace Throttlr.Api.RateLimit
{
    /// <summary>
    /// Provides persistent storage for rate limiting counters.
    /// </summary>
    internal interface IRateLimitStore
    {
        /// <summary>
        /// Retrieves the current state of a rate limit counter.
        /// </summary>
        /// <param name="key">The partition key (e.g., user, IP, route).</param>
        /// <param name="cancellationToken">Token to observe cancellation.</param>
        Task<RateLimitCounter?> GetAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the counter for the given partition key.
        /// </summary>
        /// <param name="key">The partition key.</param>
        /// <param name="counter">The updated rate limit counter.</param>
        /// <param name="cancellationToken">Token to observe cancellation.</param>
        Task SetAsync(string key, RateLimitCounter counter, CancellationToken cancellationToken = default);
    }
}
