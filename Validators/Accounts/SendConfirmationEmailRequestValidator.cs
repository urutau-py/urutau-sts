using FluentValidation;
using urutau.Models.Account;

namespace urutau.Validators.Accounts;

public sealed class SendConfirmationEmailRequestValidator : AbstractValidator<SendConfirmationEmailRequest>
{
    public SendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}