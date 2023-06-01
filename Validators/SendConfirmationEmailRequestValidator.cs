using FluentValidation;
using urutau.Models.Account;

namespace urutau.Validators;

public sealed class SendConfirmationEmailRequestValidator : AbstractValidator<SendConfirmationEmailRequest>
{
    public SendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}