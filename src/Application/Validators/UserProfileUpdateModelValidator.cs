using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class UserProfileUpdateModelValidator : AbstractValidator<UserProfileUpdateModel>
{
    public UserProfileUpdateModelValidator()
    {
        RuleFor(x => x.UserName).NotEqual(string.Empty)
            .WithMessage("Username cannot be an empty string");
        RuleFor(x => x.UserName).Length(3, 25)
            .WithMessage("Username length must be between 3 and 25 characters");
        RuleFor(x => x.Email).NotEqual(string.Empty)
            .WithMessage("Email cannot be an empty string");
        RuleFor(x => x.Email).EmailAddress()
            .WithMessage("Email is in incorrect format");
    }
}
