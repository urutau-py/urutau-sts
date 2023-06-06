using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urutau.Entities;
using urutau.Models.Shared;
using urutau.Models.User;

namespace urutau.Controllers;

[ApiController]
[Route("/api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .ProjectToType<UserResponse>()
            .ToListAsync(cancellationToken);
        
        return Ok(users);
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NoContent();
        var response = user.Adapt<UserResponse>();
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(UserPostRequest request, [FromServices] IValidator<UserPostRequest> validator, CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new DefaultResponse(false, validation.Errors.Select(x => new DefaultError(x.ErrorCode, x.ErrorMessage)).ToList()));
        }
        
        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            UserName = request.UserName,
            EmailConfirmed = request.EmailConfirmed,
            TwoFactorEnabled = request.TwoFactorEnabled,
            PhoneNumberConfirmed = request.PhoneNumberConfirmed,
            LockoutEnabled = request.LockoutEnabled
        };

        var result = string.IsNullOrWhiteSpace(request.Password) 
            ? await _userManager.CreateAsync(user) 
            : await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }
        
        return StatusCode(201, result);
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