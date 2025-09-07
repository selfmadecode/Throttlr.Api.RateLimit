using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace RateLimit.Throttlr.Middleware
{
    /// <summary>
    /// Middleware that enforces rate limiting for incoming HTTP requests.
    /// </summary>
    internal class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimiter _rateLimiter;

        private const string RetryAfter = "Retry-After";
        private const string RateLimitLimit = "X-RateLimit-Limit";
        private const string RateLimitRemaining = "X-RateLimit-Remaining";
        private const string RateLimitReset = "X-RateLimit-Reset";

        public RateLimitingMiddleware(RequestDelegate next, IRateLimiter rateLimiter)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = ResolveClientKey(context);

            var result = await _rateLimiter.ShouldLimitAsync(key, context.RequestAborted);

            if (result.IsLimited)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                // Add standard rate limit headers
                context.Response.Headers[RetryAfter] = ((int)result.RetryAfter.Value.TotalSeconds).ToString();
                context.Response.Headers[RateLimitLimit] = _rateLimiter.GetLimit().ToString();
                context.Response.Headers[RateLimitRemaining] = result.Remaining.ToString();
                context.Response.Headers[RateLimitReset] = result.Reset?.ToUnixTimeSeconds().ToString();

                await context.Response.WriteAsync("Too Many Requests");
                return;
            }

            // Attach useful headers on success
            context.Response.Headers[RateLimitLimit] = _rateLimiter.GetLimit().ToString();
            context.Response.Headers[RateLimitRemaining] = result.Remaining.ToString();
            context.Response.Headers[RateLimitReset] = result.Reset?.ToUnixTimeSeconds().ToString();

            await _next(context);
        }

        private static string ResolveClientKey(HttpContext context)
        {
            // Default: use IP address. Could be swapped for API key / user ID.
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
