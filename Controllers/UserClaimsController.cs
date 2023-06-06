using Microsoft.AspNetCore.Mvc;

namespace urutau.Controllers;

[ApiController]
[Route("/api/user-claims")]
public sealed class UserClaimsController : ControllerBase
{
    public UserClaimsController()
    {
        
    }
    
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        return Ok();
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken = default)
    {
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(CancellationToken cancellationToken = default)
    {
        return StatusCode(201);
    }
    
    [HttpPut]
    public async Task<IActionResult> Put(CancellationToken cancellationToken = default)
    {
        return Ok();
    }
    
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
    {
        return NoContent();
    }
}