using FluentValidation;
using urutau.Constants;
using urutau.Models.Account;

namespace urutau.Validators.Accounts;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^(9\d{2}|09\d{2})\d{6}$");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(PasswordConstants.MinimumLength);
    }
}