using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using RateLimit.Throttlr.Core.Counter;

namespace RateLimit.Throttlr.Core.Store
{
    /// <summary>
    /// Provides an in-memory implementation of <see cref="IRateLimitStore"/>.
    /// Suitable for single-instance deployments.
    /// </summary>
    internal class InMemoryRateLimitStore : IRateLimitStore
    {
        private readonly ConcurrentDictionary<string, RateLimitCounter> _counters;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRateLimitStore"/> class.
        /// </summary>
        public InMemoryRateLimitStore()
        {
            _counters = new ConcurrentDictionary<string, RateLimitCounter>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public Task<RateLimitCounter?> GetAsync(string key,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _counters.TryGetValue(key, out var counter);
            return Task.FromResult<RateLimitCounter?>(counter);
        }

        /// <inheritdoc />
        public Task SetAsync(string key, RateLimitCounter counter,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _counters[key] = counter;
            return Task.CompletedTask;
        }
    }
}
