using FluentValidation;
using urutau.Models.User;

namespace urutau.Validators.Users;

public sealed class UserPostRequestValidator : AbstractValidator<UserPostRequest>
{
    public UserPostRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
    }
}