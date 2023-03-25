using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;

public class RegistrationRequestModelValidator : AbstractValidator<RegistrationRequestModel>
{
    public RegistrationRequestModelValidator()
    {
        RuleFor(x => x.UserName).NotNull().NotEmpty()
            .WithMessage("UserName is required");
        RuleFor(x => x.UserName).Length(3, 25)
            .WithMessage("Username length must be between 3 and 25 characters");
        RuleFor(x => x.Email).NotNull().NotEmpty()
            .WithMessage("Email is required");
        RuleFor(x => x.Email).EmailAddress()
            .WithMessage("Email address is not in correct format");
        RuleFor(x => x.Password).NotNull().NotEmpty()
            .WithMessage("Password is required");
        RuleFor(x => x.Password).MinimumLength(8)
            .WithMessage("Password length must be at least 8 characters");
    }
}
