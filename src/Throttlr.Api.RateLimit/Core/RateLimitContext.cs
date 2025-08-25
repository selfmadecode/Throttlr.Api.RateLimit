using System.Collections.Generic;

namespace Throttlr.Api.RateLimit.Core
{
    /// <summary>
    /// Provides contextual information used for rate limiting decisions.
    /// </summary>
    internal sealed class RateLimitContext
    {
        /// <summary>
        /// Gets or sets a unique identifier for the request (e.g., client IP, user ID).
        /// </summary>
        public string RequestKey { get; set; }

        /// <summary>
        /// Gets or sets optional metadata associated with the request.
        /// </summary>
        public IDictionary<string, object> Metadata { get; set; } =
            new Dictionary<string, object>();
    }
}
