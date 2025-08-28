using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimit.Throttlr.Core
{
    /// <summary>
    /// Defines the configuration for a named rate limit policy.
    /// Immutable and safe to reuse across requests.
    /// </summary>
    public sealed class RateLimitPolicy
    {
        /// <summary>
        /// Gets the unique policy name (e.g., "api-global", "per-user").
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the maximum number of permits allowed per <see cref="Window"/>.
        /// For token bucket this is also treated as the bucket capacity.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Gets the time window over which <see cref="Limit"/> applies.
        /// For token bucket, refill rate defaults to Limit / Window.
        /// </summary>
        public TimeSpan Window { get; }

        /// <summary>
        /// Optional delegate that resolves a partition key from a request context.
        /// If null, the limiter uses <see cref="RateLimitContext.RequestKey"/>.
        /// </summary>
        public Func<RateLimitContext, string> PartitionKeySelector { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitPolicy"/> class.
        /// </summary>
        /// <param name="name">Unique policy name.</param>
        /// <param name="limit">Max permits per window (capacity for token bucket).</param>
        /// <param name="window">Window duration (must be &gt; 0).</param>
        /// <param name="partitionKeySelector">
        /// Optional delegate to resolve partition keys (e.g., per-IP, per-user).
        /// If omitted, <see cref="RateLimitContext.RequestKey"/> is used.
        /// </param>
        public RateLimitPolicy(
            string name,
            int limit,
            TimeSpan window,
            Func<RateLimitContext, string> partitionKeySelector = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Policy name cannot be null or empty.", nameof(name));
            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");
            if (window <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(window), "Window must be greater than zero.");

            Name = name;
            Limit = limit;
            Window = window;
            PartitionKeySelector = partitionKeySelector ?? (ctx => ctx?.RequestKey ?? "_");
        }

        /// <summary>
        /// Resolves the partition key for this policy given the context.
        /// </summary>
        public string ResolvePartitionKey(RateLimitContext context)
            => (PartitionKeySelector?.Invoke(context)) ?? (context?.RequestKey ?? "_");

        /// <summary>
        /// Creates a copy of this policy with a new name.
        /// </summary>
        public RateLimitPolicy WithName(string name) =>
            new RateLimitPolicy(name, Limit, Window, PartitionKeySelector);

        /// <summary>
        /// Creates a copy of this policy with a new limit.
        /// </summary>
        public RateLimitPolicy WithLimit(int limit) =>
            new RateLimitPolicy(Name, limit, Window, PartitionKeySelector);

        /// <summary>
        /// Creates a copy of this policy with a new window.
        /// </summary>
        public RateLimitPolicy WithWindow(TimeSpan window) =>
            new RateLimitPolicy(Name, Limit, window, PartitionKeySelector);

        /// <summary>
        /// Creates a copy of this policy with a new partition selector.
        /// </summary>
        public RateLimitPolicy WithPartitionSelector(Func<RateLimitContext, string> selector) =>
            new RateLimitPolicy(Name, Limit, Window, selector);
    }
}
