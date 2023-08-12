using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.WebAPI.Controllers;
using TaskTracker.WebAPI.UnitTests.Helpers;

namespace TaskTracker.WebAPI.UnitTests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _serviceMock;
    private readonly AccountController _controller;
    public AccountControllerTests()
    {
        _serviceMock = new Mock<IAccountService>();
        _controller = new AccountController(_serviceMock.Object, ControllersHelper.GetValidationService());
    }

    [Fact]
    public async Task Login_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequestModel>()))
            .ReturnsAsync(new LoginResponseModel() { Success = true });
        var model = new LoginRequestModel() { NameOrEmail = "UserName", Password = "UserPassword" };

        var result = (await _controller.Login(model)).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task Login_ReturnsLoginResponseModel()
    {
        _serviceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequestModel>()))
            .ReturnsAsync(new LoginResponseModel() { Success = true });
        var model = new LoginRequestModel() { NameOrEmail = "UserName", Password = "UserPassword" };

        var result = ((await _controller.Login(model)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<LoginResponseModel>(result);
    }
    [Fact]
    public async Task Login_ReturnsUnsuccessLoginResponseModel_IfRequestIsInvalid()
    {
        var controller = new AccountController(_serviceMock.Object,
            ControllersHelper.GetValidationService(false));

        var result = ((await controller.Login(new LoginRequestModel())).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<LoginResponseModel>(result);
        Assert.False((result as LoginResponseModel)?.Success);
    }

    [Fact]
    public async Task Registration_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(s => s.RegistrationAsync(It.IsAny<RegistrationRequestModel>()))
            .ReturnsAsync(new RegistrationResponseModel() { Success = true });
        var model = new RegistrationRequestModel() { UserName = "UserName", Email = "test@example.com", Password = "Password" };

        var result = (await _controller.Registration(model)).Result;

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Registration_ReturnsRegistrationResponseModel()
    {
        _serviceMock.Setup(s => s.RegistrationAsync(It.IsAny<RegistrationRequestModel>()))
            .ReturnsAsync(new RegistrationResponseModel() { Success = true });
        var model = new RegistrationRequestModel() { UserName = "UserName", Email = "test@example.com", Password = "Password" };

        var result = ((await _controller.Registration(model)).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<RegistrationResponseModel>(result);
    }
    [Fact]
    public async Task Login_ReturnsUnsuccessRegistrationResponseModel_IfRequestIsInvalid()
    {
        var controller = new AccountController(_serviceMock.Object,
            ControllersHelper.GetValidationService(false));

        var result = ((await controller.Registration(new RegistrationRequestModel()))
            .Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<RegistrationResponseModel>(result);
        Assert.False((result as RegistrationResponseModel)?.Success);
    }

    [Fact]
    public async Task ViewProfile_ReturnsOkObjectResult()
    {
        _serviceMock.Setup(s => s.GetUserProfileAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());
        ControllersHelper.AddAuthorizedIdentityUserToControllerContext(_controller);

        var result = (await _controller.ViewProfile()).Result;

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ViewProfile_ReturnsUserProfileModel()
    {
        _serviceMock.Setup(s => s.GetUserProfileAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());
        ControllersHelper.AddAuthorizedIdentityUserToControllerContext(_controller);

        var result = ((await _controller.ViewProfile()).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<UserProfileModel>(result);
    }

    [Fact]
    public async Task ViewProfile_ReturnsNotFoundResult_IfThereAreNoSuchAnUser()
    {
        _serviceMock.Setup(s => s.GetUserProfileAsync(It.IsAny<string>()))
            .ReturnsAsync((UserProfileModel?)null);
        ControllersHelper.AddAuthorizedIdentityUserToControllerContext(_controller);

        var result = (await _controller.ViewProfile()).Result;

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ViewProfile_ReturnsNotFoundResult_IfCalledByUnauthicatedUser()
    {
        _serviceMock.Setup(s => s.GetUserProfileAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());

        var result = (await _controller.ViewProfile()).Result;

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task ViewProfile_ReturnsNotFoundResult_IfUserIsNotIdentified()
    {
        _serviceMock.Setup(s => s.GetUserProfileAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());
        ControllersHelper.AddUserWithoutIdentityToControllerContext(_controller);

        var result = (await _controller.ViewProfile()).Result;

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsNoContentResult()
    {
        _serviceMock.Setup(s => s.UpdateUserProfileAsync(It.IsAny<string>(),
            It.IsAny<UserProfileUpdateModel>())).Callback(() => { });
        ControllersHelper.AddAuthorizedIdentityUserToControllerContext(_controller);
        var model = new UserProfileUpdateModel() { Email = "test@example.com" };

        var result = await _controller.UpdateProfile(model);

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task UpdateProfile_ReturnsNotFoundResult_IfCalledByUnauthicatedUser()
    {
        _serviceMock.Setup(s => s.UpdateUserProfileAsync(It.IsAny<string>(),
            It.IsAny<UserProfileUpdateModel>())).Callback(() => { });
        var model = new UserProfileUpdateModel() { Email = "test@example.com" };

        var result = await _controller.UpdateProfile(model);

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task UpdateProfile_ReturnsNotFoundResult_IfUserIsNotIdentified()
    {
        _serviceMock.Setup(s => s.UpdateUserProfileAsync(It.IsAny<string>(),
            It.IsAny<UserProfileUpdateModel>())).Callback(() => { });
        ControllersHelper.AddUserWithoutIdentityToControllerContext(_controller);
        var model = new UserProfileUpdateModel() { Email = "test@example.com" };

        var result = await _controller.UpdateProfile(model);

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task UpdateProfile_ReturnsBadRequestObjectResult_IfModelWasInvalid()
    {
        var controller = new AccountController(_serviceMock.Object,
            ControllersHelper.GetValidationService(false));

        var result = await controller.UpdateProfile(new UserProfileUpdateModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task UpdateProfile_ReturnsBadRequestObjectResult_IfUpdatingWasNotSuccessful()
    {
        _serviceMock.Setup(s => s.UpdateUserProfileAsync(It.IsAny<string>(),
            It.IsAny<UserProfileUpdateModel>()))
            .ThrowsAsync(new ArgumentException("TestException"));
        ControllersHelper.AddAuthorizedIdentityUserToControllerContext(_controller);
        var model = new UserProfileUpdateModel() { Email = "test@example.com" };

        var result = await _controller.UpdateProfile(model);

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task ChangePassword_ReturnsNoContentResult()
    {
        _serviceMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(),
            It.IsAny<ChangePasswordModel>())).Callback(() => { });
        ControllersHelper.AddAuthorizedIdentityUserToControllerContext(_controller);
        var model = new ChangePasswordModel() { OldPassword = "oldPassword", NewPassword = "newPassword" };

        var result = await _controller.ChangePassword(model);

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task ChangePassword_ReturnsNotFoundResult_IfCalledByUnauthicatedUser()
    {
        _serviceMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(),
            It.IsAny<ChangePasswordModel>())).Callback(() => { });
        var model = new ChangePasswordModel() { OldPassword = "oldPassword", NewPassword = "newPassword" };

        var result = await _controller.ChangePassword(model);

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task ChangePassword_ReturnsNotFoundResult_IfUserIsNotIdentified()
    {
        _serviceMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(),
            It.IsAny<ChangePasswordModel>())).Callback(() => { });
        ControllersHelper.AddUserWithoutIdentityToControllerContext(_controller);
        var model = new ChangePasswordModel();

        var result = await _controller.ChangePassword(model);

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task ChangePassword_ReturnsBadRequestObjectResult_IfModelWasInvalid()
    {
        var controller = new AccountController(_serviceMock.Object,
            ControllersHelper.GetValidationService(false));

        var result = await controller.ChangePassword(new ChangePasswordModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task ChangePassword_ReturnsBadRequestObjectResult_IfOperationWasNotSuccessful()
    {
        _serviceMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(),
            It.IsAny<ChangePasswordModel>()))
            .ThrowsAsync(new ArgumentException("TestException"));
        ControllersHelper.AddAuthorizedIdentityUserToControllerContext(_controller);
        var model = new ChangePasswordModel() { OldPassword = "oldPassword", NewPassword = "newPassword" };

        var result = await _controller.ChangePassword(model);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}