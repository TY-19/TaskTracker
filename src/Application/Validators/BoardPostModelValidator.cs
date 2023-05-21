using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class BoardPostModelValidator : AbstractValidator<BoardPostModel>
{
    public BoardPostModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("First name is required");
    }
}
