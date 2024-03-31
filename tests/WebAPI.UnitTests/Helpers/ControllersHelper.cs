using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;

namespace TaskTracker.WebAPI.UnitTests.Helpers;

internal static class ControllersHelper
{
    private static ValidationResult ValidationSuccessResult => new();
    private static ValidationResult ValidationFailResult => new()
    {
        Errors = new List<ValidationFailure>()
        {
            new ValidationFailure()
        }
    };
    public static IValidationService GetValidationService(bool isModelValid = true)
    {
        var _validationServiceMock = new Mock<IValidationService>();
        ValidationResult validationResult = isModelValid
            ? ValidationSuccessResult
            : ValidationFailResult;
        _validationServiceMock.Setup(v => v.Validate(It.IsAny<IValidatableModel>()))
                .Returns(validationResult);
        return _validationServiceMock.Object;
    }
    public static void AddAuthorizedIdentityUserToControllerContext(ControllerBase controller)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "User")
        }, "mock"));

        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }
    public static void AddUserWithoutIdentityToControllerContext(ControllerBase controller)
    {
        var user = new ClaimsPrincipal();

        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }
}
