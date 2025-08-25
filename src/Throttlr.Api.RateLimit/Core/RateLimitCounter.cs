using System;
using System.Collections.Generic;
using System.Text;

namespace Throttlr.Api.RateLimit.Core
{
    /// <summary>
    /// Represents the state of a rate limit counter.
    /// </summary>
    internal sealed class RateLimitCounter
    {
        /// <summary>
        /// Gets or sets the number of requests made in the current window.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last request in this window.
        /// </summary>
        public long TimestampUtc { get; set; }
    }
}
