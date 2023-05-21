using FluentValidation;
using TaskTracker.Application.Models;

namespace TaskTracker.Application.Validators;
public class BoardPutModelValidator : AbstractValidator<BoardPutModel>
{
    public BoardPutModelValidator()
    {
        RuleFor(x => x.Name).NotEqual(string.Empty)
            .WithMessage("Board name cannot be an empty string");
    }
}
