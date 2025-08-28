using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimit.Throttlr.Core.Counter
{
    /// <summary>
    /// Represents a token bucket counter.
    /// </summary>
    public sealed class TokenBucketCounter
    {
        /// <summary>
        /// Gets the number of tokens remaining.
        /// </summary>
        public int TokensRemaining { get; }

        /// <summary>
        /// Gets the timestamp of the last refill.
        /// </summary>
        public DateTimeOffset LastRefill { get; }

        /// <summary>
        /// Gets the expiration timestamp for this bucket.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucketCounter"/> class.
        /// </summary>
        public TokenBucketCounter(int tokensRemaining, DateTimeOffset lastRefill, DateTimeOffset expiresAt)
        {
            TokensRemaining = tokensRemaining;
            LastRefill = lastRefill;
            ExpiresAt = expiresAt;
        }

        /// <summary>
        /// Refill tokens based on elapsed time.
        /// </summary>
        public TokenBucketCounter Refill(DateTimeOffset now, RateLimitPolicy policy)
        {
            var elapsed = now - LastRefill;

            // how many tokens to add based on rate
            var refillRatePerMs = (double)policy.Limit / policy.Window.TotalMilliseconds;
            var tokensToAdd = (int)(elapsed.TotalMilliseconds * refillRatePerMs);

            if (tokensToAdd <= 0)
            {
                return this;
            }

            var newCount = Math.Min(TokensRemaining + tokensToAdd, policy.Limit);

            return new TokenBucketCounter(newCount, now, now.Add(policy.Window));
        }

        /// <summary>
        /// Consumes one token.
        /// </summary>
        public TokenBucketCounter Consume()
            => new TokenBucketCounter(TokensRemaining - 1, LastRefill, ExpiresAt);

        /// <summary>
        /// Calculates when the next token will be available.
        /// </summary>
        public TimeSpan NextAvailableIn(DateTimeOffset now, RateLimitPolicy policy)
        {
            if (TokensRemaining > 0)
            {
                return TimeSpan.Zero;
            }

            var refillRatePerMs = (double)policy.Limit / policy.Window.TotalMilliseconds;
            var msUntilNext = (1.0 / refillRatePerMs);
            return TimeSpan.FromMilliseconds(msUntilNext);
        }
    }
}
