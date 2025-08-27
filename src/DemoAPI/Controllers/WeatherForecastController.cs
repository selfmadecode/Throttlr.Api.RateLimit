using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            return Ok(new
            {
                message = "Hello, world!",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("time")]
        public IActionResult GetTime()
        {
            return Ok(new
            {
                serverTime = DateTime.UtcNow,
                note = "This endpoint is rate limited!"
            });
        }
    }
}
