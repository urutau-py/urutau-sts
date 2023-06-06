using FluentValidation;
using urutau.Models.Account;

namespace urutau.Validators.Accounts;

public sealed class GenerateResetPasswordTokenRequestValidator : AbstractValidator<GenerateResetPasswordTokenRequest>
{
    public GenerateResetPasswordTokenRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}