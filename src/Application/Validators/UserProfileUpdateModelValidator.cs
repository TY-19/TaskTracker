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
        RuleFor(x => x.UserName).Matches("^[a-zA-Z0-9_]*$")
            .WithMessage("Username may contain only letters, digits or undescore");
        RuleFor(x => x.Email).NotEqual(string.Empty)
            .WithMessage("Email cannot be an empty string");
        RuleFor(x => x.Email).EmailAddress()
            .WithMessage("Email is in incorrect format");
        RuleFor(x => x.FirstName).NotEqual(string.Empty)
            .WithMessage("First name cannot be an empty string");
        RuleFor(x => x.FirstName).MaximumLength(50)
                .WithMessage("First name length must be less than 50 characters");
        RuleFor(x => x.LastName).NotEqual(string.Empty)
            .WithMessage("Last name cannot be an empty string");
        RuleFor(x => x.LastName).MaximumLength(50)
            .WithMessage("Last name length must be less than 50 characters");
    }
}
