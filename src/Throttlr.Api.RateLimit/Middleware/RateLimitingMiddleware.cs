using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RateLimit.Throttlr.Core;

namespace RateLimit.Throttlr.Middleware
{
    /// <summary>
    /// Middleware that enforces rate limiting for incoming HTTP requests.
    /// </summary>
    internal class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimiter _rateLimiter;

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
                context.Response.Headers["Retry-After"] = ((int)result.RetryAfter.Value.TotalSeconds).ToString();
                context.Response.Headers["X-RateLimit-Limit"] = _rateLimiter.GetLimit().ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = result.Remaining.ToString();
                context.Response.Headers["X-RateLimit-Reset"] = result.Reset?.ToUnixTimeSeconds().ToString();

                await context.Response.WriteAsync("Too Many Requests");
                return;
            }

            // Attach useful headers on success
            context.Response.Headers["X-RateLimit-Limit"] = _rateLimiter.GetLimit().ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = result.Remaining.ToString();
            context.Response.Headers["X-RateLimit-Reset"] = result.Reset?.ToUnixTimeSeconds().ToString();

            await _next(context);
        }

        private static string ResolveClientKey(HttpContext context)
        {
            // Default: use IP address. Could be swapped for API key / user ID.
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        ///// <summary>
        ///// Processes the HTTP request, applying rate limiting.
        ///// </summary>
        //public async Task InvokeAsync(HttpContext context)
        //{
        //    var requestKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        //    var rateLimitContext = new RateLimitContext(requestKey);
        //    var result = await _rateLimiter.CheckAsync(rateLimitContext, context.RequestAborted);

        //    if (!result.Allowed)
        //    {
        //        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        //        if (result.RetryAfter.HasValue)
        //        {
        //            context.Response.Headers["Retry-After"] =
        //                ((int)result.RetryAfter.Value.TotalSeconds).ToString();
        //        }

        //        if (result.Reset.HasValue)
        //        {
        //            context.Response.Headers["X-RateLimit-Reset"] =
        //                ((int)result.Reset.Value.TotalSeconds).ToString();
        //        }

        //        context.Response.Headers["X-RateLimit-Remaining"] = "0";
        //        return;
        //    }

        //    context.Response.Headers["X-RateLimit-Remaining"] =
        //        result.Remaining?.ToString() ?? "unknown";

        //    await _next(context);
        //}
    }
}
