using FluentValidation.Results;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;

namespace TaskTracker.WebAPI.UnitTests.Helpers;

internal static class ControllersHelper
{
    public static IValidationService GetValidationService()
    {
        var _validationServiceMock = new Mock<IValidationService>();
        _validationServiceMock.Setup(v => v.Validate(It.IsAny<IValidatableModel>()))
            .Returns(new ValidationResult());
        return _validationServiceMock.Object;
    }
}
