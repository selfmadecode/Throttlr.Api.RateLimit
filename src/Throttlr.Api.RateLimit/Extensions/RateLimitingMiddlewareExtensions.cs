using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using RateLimit.Throttlr.Abstractions;
using RateLimit.Throttlr.Core;
using RateLimit.Throttlr.Core.Store;
using RateLimit.Throttlr.Middleware;

namespace RateLimit.Throttlr.Extensions
{
    /// <summary>
    /// Provides extension methods to register and use the rate limiting middleware.
    /// </summary>
    public static class RateLimitingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="RateLimitingMiddleware"/> to the request pipeline.
        /// </summary>
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RateLimitingMiddleware>();
        }

        /// <summary>
        /// Registers rate limiting services with default in-memory store and system clock.
        /// </summary>
        public static IServiceCollection AddRateLimiting(
            this IServiceCollection services,
            RateLimitPolicy policy,
            RateLimiterType limiterType = RateLimiterType.FixedWindow)
        {
            services.AddSingleton<ISystemClock, SystemClock>();
            services.AddSingleton<IRateLimitStore, InMemoryRateLimitStore>();
            services.AddSingleton(policy);

            // Factory: choose limiter type
            services.AddSingleton<IRateLimiter>(sp =>
            {
                var store = sp.GetRequiredService<IRateLimitStore>();
                var clock = sp.GetRequiredService<ISystemClock>();
                var p = sp.GetRequiredService<RateLimitPolicy>();

                return limiterType switch
                {
                    RateLimiterType.FixedWindow => new FixedWindowRateLimiter(store, clock, p),
                    RateLimiterType.SlidingWindow => new SlidingWindowRateLimiter(store, clock, p),
                    RateLimiterType.TokenBucket => new TokenBucketRateLimiter(store, clock, p),
                    _ => new FixedWindowRateLimiter(store, clock, p)
                };
            });

            return services;
        }

        ///// <summary>
        ///// Adds in-memory rate limiting with a specified policy to the DI container.
        ///// </summary>
        //public static IServiceCollection AddInMemoryRateLimiting(
        //    this IServiceCollection services,
        //    RateLimitPolicy policy,
        //    RateLimiterType type = RateLimiterType.TokenBucket)
        //{
        //    services.AddSingleton<ISystemClock, SystemClock>();
        //    services.AddSingleton<IRateLimitStore, InMemoryRateLimitStore>();

        //    // Choose limiter
        //    services.AddSingleton<IRateLimiter>(sp =>
        //    {
        //        var store = sp.GetRequiredService<IRateLimitStore>();
        //        var clock = sp.GetRequiredService<ISystemClock>();

        //        return type switch
        //        {
        //            RateLimiterType.FixedWindow => new FixedWindowRateLimiter(store, policy, clock),
        //            RateLimiterType.TokenBucket => new TokenBucketRateLimiter(store, policy, clock),
        //            RateLimiterType.SlidingWindow => new SlidingWindowRateLimiter(store, policy, clock),
        //            _ => new TokenBucketRateLimiter(store, policy, clock)
        //        };
        //    });

        //    return services;
        //}
    }
    /// <summary>
    /// Supported rate limiter types.
    /// </summary>
    public enum RateLimiterType
    {
        FixedWindow,
        TokenBucket,
        SlidingWindow
    }
}
