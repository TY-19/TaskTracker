using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class LoginRequestModelValidator : AbstractValidator<LoginRequestModel>
{
    public LoginRequestModelValidator()
    {
        RuleFor(x => x.NameOrEmail).NotEmpty()
            .WithMessage("Name/email is required");
        RuleFor(x => x.Password).NotEmpty()
            .WithMessage("Password is required");
    }
}
