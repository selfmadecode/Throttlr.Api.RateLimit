# RateLimit.Throttlr

[![NuGet Version](https://img.shields.io/nuget/v/RateLimit.Throttlr.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RateLimit.Throttlr)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RateLimit.Throttlr.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RateLimit.Throttlr)
[![License](https://img.shields.io/github/license/selfmadecode/Throttlr.Api.RateLimit.svg?style=flat)](LICENSE)
[![Build](https://img.shields.io/github/actions/workflow/status/selfmadecode/Throttlr.Api.RateLimit/build.yml?branch=main&style=flat&logo=github)](https://github.com/selfmadecode/Throttlr.Api.RateLimit/actions)
[![.NET Standard](https://img.shields.io/badge/.NET%20Standard-2.1-blue)](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-1)
[![Can be used by .NET](https://img.shields.io/badge/Can%20be%20used%20by-.NET%205%2C%206%2C%207%2C%208-blueviolet)](https://dotnet.microsoft.com/)
[![GitHub issues](https://img.shields.io/github/issues/selfmadecode/Throttlr.Api.RateLimit.svg?style=flat)](https://github.com/selfmadecode/Throttlr.Api.RateLimit/issues)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat)](https://github.com/selfmadecode/Throttlr.Api.RateLimit/pulls)

A flexible and customizable **rate limiting library** for modern and legacy **.NET APIs**.

---

## Overview
RateLimit.Throttlr supports multiple rate limiting algorithms:  

- **Fixed Window**  
- **Sliding Window**  
- **Token Bucket**  

---

## Installation

Install via NuGet:

```bash
   dotnet add package RateLimit.Throttlr
   ```

## Usage Example
### Register in DI
```csharp
using RateLimit.Throttlr.Core;
using RateLimit.Throttlr.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add rate limiting services
builder.Services.AddRateLimiting(
    new RateLimitPolicy(
        name: "global",                        // unique policy name
        limit: 5,                               // max 5 requests
        window: TimeSpan.FromSeconds(10)       // per 10 seconds
    ),
    limiterType: RateLimiterType.SlidingWindow // Options: FixedWindow | SlidingWindow | TokenBucket
);

```

### Configure Middleware

```csharp
app.UseRouting();

app.UseRateLimiting(); // Apply rate limiting

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

```
## Example Controller
```csharp
namespace DemoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            return Ok(new
            {
                message = "Hello, world!",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
```

### Behavior:

Each IP (or custom partition key) can only make 5 requests per 10 seconds.
Exceeding the limit returns HTTP 429 Too Many Requests.
Standard headers:
X-RateLimit-Limit: total allowed requests
X-RateLimit-Remaining: requests left in the current window
X-RateLimit-Reset: Unix timestamp when window resets
Retry-After: seconds to wait before next allowed request

## Supported Algorithms

Limiter Type	Description
FixedWindow	Counts requests per fixed window interval
SlidingWindow	Smooths counts using weighted previous window
TokenBucket	Allows bursts and steady refill rate