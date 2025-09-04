using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
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
