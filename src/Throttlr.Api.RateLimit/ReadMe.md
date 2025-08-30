# RateLimit.Throttlr

[![NuGet Version](https://img.shields.io/nuget/v/RateLimit.Throttlr.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RateLimit.Throttlr)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RateLimit.Throttlr.svg?style=flat&logo=nuget)](https://www.nuget.org/packages/RateLimit.Throttlr)
[![License](https://img.shields.io/github/license/yourusername/RateLimit.Throttlr.svg?style=flat)](LICENSE)
[![Build](https://img.shields.io/github/actions/workflow/status/yourusername/RateLimit.Throttlr/dotnet.yml?branch=main&style=flat&logo=github)](https://github.com/yourusername/RateLimit.Throttlr/actions)

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

