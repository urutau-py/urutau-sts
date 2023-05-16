using Microsoft.AspNetCore.Mvc;

namespace urutau.Controllers;

[ApiController]
[Route("api/health-check")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok("ok");
    }
}