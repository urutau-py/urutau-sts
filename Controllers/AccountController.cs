using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using urutau.Entities;
using urutau.Models.Account;
using urutau.Models.Shared;
using urutau.Models.User;

namespace urutau.Controllers;

[ApiController]
[Route("api/account")]
// [Authorize]
public sealed class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult<DefaultResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };

        if (string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new DefaultResponse(false, "Password is required")); 
        }
        
        var passwordErrors = await ValidatePassword(user, request.Password);
        
        if (passwordErrors.Any())
        {
            return BadRequest(new DefaultResponse(false, passwordErrors));
        }
        
        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new DefaultResponse(false, result.Errors.Select(x => x.Description).ToList()));
        }

        
        var passwordResult = await _userManager.AddPasswordAsync(user, request.Password);
        if (!passwordResult.Succeeded)
        {
            return BadRequest(new DefaultResponse(passwordResult.Succeeded, passwordResult.Errors.Select(x => x.Description).ToList()));
        }

        var userResponse = new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email
        };
        
        return Ok(new DefaultResponse<UserResponse>(true, userResponse));   
    }

    [HttpPost("reset-password")]
    public ActionResult<DefaultResponse> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        return Ok();
    }
    
    [HttpPost("change-password")]
    public ActionResult<DefaultResponse> ChangePassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        return Ok();
    }
    
    private async Task<List<string>> ValidatePassword(ApplicationUser user, string newPassword)
    {
        var passwordErrors = new List<string>();

        var validators = _userManager.PasswordValidators;

        foreach(var validator in validators)
        {
            var result = await validator.ValidateAsync(_userManager, user, newPassword);

            if (result.Succeeded) continue;
            passwordErrors.AddRange(result.Errors.Select(error => error.Description));
        }

        return passwordErrors;
    }
}