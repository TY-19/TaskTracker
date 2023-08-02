using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
{
    public ChangePasswordModelValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty()
            .WithMessage("OldPassword is required");
        RuleFor(x => x.NewPassword).NotEmpty()
            .WithMessage("NewPassword is required");
        RuleFor(x => x.NewPassword).MinimumLength(8)
            .WithMessage("Password length must be at least 8 characters");
        RuleFor(x => x.NewPassword).MaximumLength(20)
            .WithMessage("Password length must be less than 20 characters");
        RuleFor(x => x.NewPassword).NotEqual(x => x.OldPassword)
            .WithMessage("New password cannot be the same as the old one");
    }
}
