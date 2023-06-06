using System.Text.Encodings.Web;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urutau.Entities;
using urutau.Models.Account;
using urutau.Models.Shared;
using urutau.Models.User;
using urutau.Services.Interfaces;
using urutau.Validators;

namespace urutau.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    [HttpPost("register")]
    public async Task<ActionResult<DefaultResponse>> Register([FromServices] IValidator<RegisterRequest> validator,
        [FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new DefaultResponse(false, validation.Errors.Select(x => new DefaultError(x.ErrorCode, x.ErrorMessage)).ToList()));
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };
        
        var passwordErrors = await ValidatePassword(user, request.Password);

        if (passwordErrors.Any())
        {
            return BadRequest(new DefaultResponse(false, passwordErrors));
        }

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new DefaultResponse(false, result.Errors.Select(x => new DefaultError(x.Code, x.Description)).ToList()));
        }


        var passwordResult = await _userManager.AddPasswordAsync(user, request.Password);
        if (!passwordResult.Succeeded)
        {
            return BadRequest(new DefaultResponse(passwordResult.Succeeded,
                passwordResult.Errors.Select(x => new DefaultError(x.Code, x.Description)).ToList()));
        }

        await SendConfirmationEmail(user, cancellationToken);
        
        var userResponse = new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email
        };

        return StatusCode(201, new DefaultResponse<UserResponse>(true, userResponse));
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<DefaultResponse>> ChangePassword([FromServices] IValidator<ChangePasswordRequest> validator, [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new DefaultResponse(false, validation.Errors.Select(x => new DefaultError(x.ErrorCode, x.ErrorMessage)).ToList()));
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null)
            return BadRequest(new DefaultResponse(false, new DefaultError("userNotFound", "User not found")));

        var result = await _userManager.GeneratePasswordResetTokenAsync(user);

        var response = new DefaultResponse<ResetPasswordResponse>(true, new ResetPasswordResponse
        {
            Token = result,
            UserId = user.Id
        });
        
        return Ok(response);
    }
    
    [HttpPost("generate-reset-password-token")]
    public async Task<ActionResult<DefaultResponse>> GeneratePasswordResetToken([FromServices] IValidator<GenerateResetPasswordTokenRequest> validator, [FromBody] GenerateResetPasswordTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new DefaultResponse(false, validation.Errors.Select(x => new DefaultError(x.ErrorCode, x.ErrorMessage)).ToList()));
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return BadRequest(new DefaultResponse(false, new List<DefaultError> {new DefaultError("userNotFound", "User not found")}));

        var result = await _userManager.GeneratePasswordResetTokenAsync(user);

        var response = new DefaultResponse<ResetPasswordResponse>(true, new ResetPasswordResponse
        {
            Token = result,
            UserId = user.Id
        });
        
        return Ok(response);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<DefaultResponse>> ChangePassword([FromServices] IValidator<ResetPasswordRequest> validator, [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new DefaultResponse(false, validation.Errors.Select(x => new DefaultError(x.ErrorCode, x.ErrorMessage)).ToList()));
        }
        
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null)
            return BadRequest(new DefaultResponse(false, new List<DefaultError> { new DefaultError("userNotFound", "User not found")}));

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        var response = new DefaultResponse(result.Succeeded, 
            result.Succeeded ? new List<DefaultError>() : result.Errors.Select(x => new DefaultError(x.Code, x.Description)).ToList());
        
        return Ok(response);
    }
    
    [HttpPost("send-confirmation-email")]
    public async Task<ActionResult<DefaultResponse>> SendConfirmationEmail([FromServices] IValidator<SendConfirmationEmailRequest> validator, [FromBody] SendConfirmationEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new DefaultResponse(false, validation.Errors.Select(x => new DefaultError(x.ErrorCode, x.ErrorMessage)).ToList()));
        }
        
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null)
            return BadRequest(new DefaultResponse(false, new List<DefaultError> {new DefaultError("userNotFound", "User not found")}));

        if (string.IsNullOrWhiteSpace(user.Email))
            return BadRequest(new DefaultResponse(false, new List<DefaultError> {new DefaultError("invalidEmail", "User does not have an email") }));

        var result = await SendConfirmationEmail(user, cancellationToken);
        
        return Ok(new DefaultResponse(result));
    }


    private async Task<bool> SendConfirmationEmail(ApplicationUser user, CancellationToken cancellationToken)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { userId = user.Id, code = token },
            protocol: Request.Scheme)!;

        if (string.IsNullOrWhiteSpace(user.Email)) return false;
        
        return await _emailSender.SendAsync(
            user.Email,
            "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", cancellationToken);
    }
    
    private async Task<List<DefaultError>> ValidatePassword(ApplicationUser user, string newPassword)
    {
        var passwordErrors = new List<DefaultError>();

        var validators = _userManager.PasswordValidators;

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(_userManager, user, newPassword);

            if (result.Succeeded) continue;
            passwordErrors.AddRange(result.Errors.Select(error => new DefaultError(error.Code, error.Description)));
        }

        return passwordErrors;
    }
}