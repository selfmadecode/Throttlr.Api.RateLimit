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
    /// A token-bucket rate limiter implementation.
    /// Allows bursts up to the bucket capacity while refilling tokens over time.
    /// </summary>
    public sealed class TokenBucketRateLimiter : IRateLimiter
    {
        private readonly IRateLimitStore _store;
        private readonly ISystemClock _clock;
        private readonly RateLimitPolicy _policy;
        private readonly int _capacity;
        private readonly double _tokensPerSecond;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucketRateLimiter"/> class.
        /// </summary>
        /// <param name="store">The rate limit store.</param>
        /// <param name="policy">The rate limit policy.</param>
        /// <param name="clock">The system clock.</param>
        public TokenBucketRateLimiter(
            IRateLimitStore store,
            ISystemClock clock,
            RateLimitPolicy policy)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));

            // Capacity = max number of tokens available at once
            _capacity = policy.Limit;

            // Token refill rate per second
            _tokensPerSecond = policy.Limit / policy.Window.TotalSeconds;
        }

        /// <inheritdoc />
        public async Task<RateLimitResult> ShouldLimitAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

            var now = _clock.UtcNow;

            var counter = await _store.GetAsync(key, cancellationToken).ConfigureAwait(false);

            int tokens;
            DateTimeOffset resetTime;

            if (counter == null)
            {
                // Bucket starts full
                tokens = _capacity - 1;
                resetTime = now.Add(_policy.Window);
            }
            else
            {
                // Time since last refill
                var elapsedSeconds = (now - counter.ResetTime).TotalSeconds;
                var refill = (int)(elapsedSeconds * _tokensPerSecond);

                // Add tokens back, capped at capacity
                tokens = Math.Min(_capacity, counter.Count + refill);

                if (tokens > 0)
                {
                    tokens--; // consume one token
                }

                // Reset time = when the bucket will be full again
                var secondsToFull = (_capacity - tokens) / _tokensPerSecond;
                resetTime = now.AddSeconds(secondsToFull);
            }

            var newCounter = new RateLimitCounter(tokens, now);
            await _store.SetAsync(key, newCounter, cancellationToken).ConfigureAwait(false);

            if (tokens < 0)
            {
                var retryAfter = TimeSpan.FromSeconds(1 / _tokensPerSecond);
                return RateLimitResult.Limited(
                    remaining: 0,
                    reset: resetTime,
                    retryAfter: retryAfter);
            }

            return RateLimitResult.Success(
                remaining: tokens,
                reset: resetTime);
        }

        public int GetLimit() => _capacity;
    }
}
