using FluentValidation;
using urutau.Constants;
using urutau.Models.Account;

namespace urutau.Validators;

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(PasswordConstants.MinimumLength);
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(PasswordConstants.MinimumLength);
    }
}