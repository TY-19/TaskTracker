using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class SetPasswordModelValidator : AbstractValidator<SetPasswordModel>
{
    public SetPasswordModelValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty()
            .WithMessage("New password is required");
        RuleFor(x => x.NewPassword).MinimumLength(8)
            .WithMessage("New password length must be at least 8 characters");
        RuleFor(x => x.NewPassword).MaximumLength(20)
            .WithMessage("New password length must be less than 20 characters");
    }
}
