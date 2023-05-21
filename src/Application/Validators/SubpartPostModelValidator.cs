using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class SubpartPostModelValidator : AbstractValidator<SubpartPostModel>
{
    public SubpartPostModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required");
        RuleFor(x => x.AssignmentId).NotEmpty()
            .WithMessage("Id of the assignment is required");
    }
}
