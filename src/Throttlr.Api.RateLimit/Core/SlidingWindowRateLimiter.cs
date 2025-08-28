using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RateLimit.Throttlr.Abstractions;
using RateLimit.Throttlr.Core.Counter;

namespace RateLimit.Throttlr.Core
{
    /// <summary>
    /// Implements a sliding-window rate limiting algorithm.
    /// Example: allow 100 requests per 1 minute window with smoother distribution.
    /// </summary>
    internal class SlidingWindowRateLimiter : IRateLimiter
    {
        private readonly IRateLimitStore _store;
        private readonly ISystemClock _clock;
        private readonly RateLimitPolicy _policy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlidingWindowRateLimiter"/> class.
        /// </summary>
        public SlidingWindowRateLimiter(
            IRateLimitStore store,
            ISystemClock clock,
            RateLimitPolicy policy)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        /// <inheritdoc />
        /// <inheritdoc />
        public async Task<RateLimitResult> ShouldLimitAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            var now = _clock.UtcNow;
            var windowSize = _policy.Window;

            // Current and previous windows
            var currentWindowStart = new DateTimeOffset(
                now.Ticks - (now.Ticks % windowSize.Ticks),
                TimeSpan.Zero);

            var previousWindowStart = currentWindowStart - windowSize;

            // Get counters
            var currentCounter = await _store.GetAsync(key + ":current", cancellationToken).ConfigureAwait(false);
            var previousCounter = await _store.GetAsync(key + ":previous", cancellationToken).ConfigureAwait(false);

            // Expire old counters
            if (previousCounter != null && previousCounter.ResetTime < now)
                previousCounter = null;

            if (currentCounter == null || currentCounter.ResetTime < now)
            {
                currentCounter = new RateLimitCounter(
                    count: 1,
                    resetTime: currentWindowStart.Add(windowSize));
            }
            else
            {
                currentCounter = new RateLimitCounter(
                    count: currentCounter.Count + 1,
                    resetTime: currentCounter.ResetTime);
            }

            // Store updated counters
            await _store.SetAsync(key + ":current", currentCounter, cancellationToken).ConfigureAwait(false);
            if (previousCounter != null)
            {
                await _store.SetAsync(key + ":previous", previousCounter, cancellationToken).ConfigureAwait(false);
            }

            // Weighted count = current window + fraction of previous window
            var elapsedTicks = now.Ticks - currentWindowStart.Ticks;
            var windowFraction = (double)elapsedTicks / windowSize.Ticks;
            var weightedPrevious = previousCounter?.Count * (1 - windowFraction) ?? 0;
            var totalCount = currentCounter.Count + weightedPrevious;

            if (totalCount > _policy.Limit)
            {
                var retryAfter = currentCounter.ResetTime - now;
                return RateLimitResult.Limited(
                    remaining: 0,
                    reset: currentCounter.ResetTime,
                    retryAfter: retryAfter);
            }

            return RateLimitResult.Success(
                remaining: _policy.Limit - (int)Math.Ceiling(totalCount),
                reset: currentCounter.ResetTime);
        }

        public int GetLimit() => _policy.Limit;
    }
}
