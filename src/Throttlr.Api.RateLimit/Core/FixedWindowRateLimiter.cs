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
    /// A fixed-window rate limiter implementation.
    /// Counts requests within a fixed time interval.
    /// </summary>
    internal class FixedWindowRateLimiter : IRateLimiter
    {
        private readonly IRateLimitStore _store;
        private readonly ISystemClock _clock;
        private readonly RateLimitPolicy _policy;

        public FixedWindowRateLimiter(
            IRateLimitStore store,
            ISystemClock clock,
            RateLimitPolicy policy)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        /// <inheritdoc />
        public async Task<RateLimitResult> ShouldLimitAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            var now = _clock.UtcNow;
            var windowStart = new DateTimeOffset(
                now.Ticks - (now.Ticks % _policy.Window.Ticks),
                TimeSpan.Zero);

            var counter = await _store.GetAsync(key, cancellationToken).ConfigureAwait(false);

            if (counter == null || counter.ResetTime < now)
            {
                // New window → start fresh
                counter = new RateLimitCounter(
                    count: 1,
                    resetTime: windowStart.Add(_policy.Window));
            }
            else
            {
                // Increment within the same window
                counter = new RateLimitCounter(
                count: counter.Count + 1,
                resetTime: counter.ResetTime);
            }

            await _store.SetAsync(key, counter, cancellationToken).ConfigureAwait(false);

            if (counter.Count > _policy.Limit)
            {
                // Blocked request
                var retryAfter = counter.ResetTime - now;
                return RateLimitResult.Limited(
                    remaining: 0,
                    reset: counter.ResetTime,
                    retryAfter: retryAfter);
            }

            // Allowed request
            return RateLimitResult.Success(
                remaining: _policy.Limit - counter.Count,
                reset: counter.ResetTime);
        }

        public int GetLimit() => _policy.Limit;
    }
}
