using Microsoft.AspNetCore.Mvc;

namespace urutau.Controllers;

[ApiController]
[Route("api/health-check")]
public sealed class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok("ok");
    }
    
    [HttpPost("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}