using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class SetPasswordModelValidator : AbstractValidator<SetPasswordModel>
{
    public SetPasswordModelValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty()
            .WithMessage("New password is required");
    }
}
