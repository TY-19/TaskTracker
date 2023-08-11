using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.WebAPI.Controllers;
using TaskTracker.WebAPI.UnitTests.Helpers;

namespace TaskTracker.WebAPI.UnitTests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly UsersController _controller;
    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _accountServiceMock = new Mock<IAccountService>();
        _controller = new UsersController(_userServiceMock.Object, _accountServiceMock.Object,
            ControllersHelper.GetValidationService());
    }
    [Fact]
    public async Task GetAllUsers_ReturnsOkObjectResult()
    {
        _userServiceMock.Setup(u => u.GetAllUsersAsync())
            .ReturnsAsync(new List<UserProfileModel>());

        var result = (await _controller.GetAllUsers()).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetAllUsers_ReturnsEnumerableOfUserProfileModels()
    {
        _userServiceMock.Setup(u => u.GetAllUsersAsync())
            .ReturnsAsync(new List<UserProfileModel>());

        var result = ((await _controller.GetAllUsers()).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<UserProfileModel>>(result);
    }
    [Fact]
    public async Task GetUserProfile_ReturnsOkObjectResult()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());

        var result = (await _controller.GetUserProfile("1")).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task GetUserProfile_ReturnsUserProfileModel()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());

        var result = ((await _controller.GetUserProfile("1")).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<UserProfileModel>(result);
    }
    [Fact]
    public async Task GetUserProfile_ReturnsNotFoundResult()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync((UserProfileModel?)null);

        var result = (await _controller.GetUserProfile("1")).Result;

        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public async Task CreateUserProfile_ReturnsOkObjectResult()
    {
        _accountServiceMock.Setup(a => a.RegistrationAsync(It.IsAny<RegistrationRequestModel>()))
            .ReturnsAsync(new RegistrationResponseModel() { Success = true });

        var result = (await _controller.CreateUserProfile(new RegistrationRequestModel())).Result;

        Assert.IsType<OkObjectResult>(result);
    }
    [Fact]
    public async Task CreateUserProfile_ReturnsUnsuccessRegistrationResponseModel_IfRequestIsInvalid()
    {
        var controller = new UsersController(_userServiceMock.Object, _accountServiceMock.Object,
            ControllersHelper.GetValidationService(false));

        var result = ((await controller.CreateUserProfile(new RegistrationRequestModel())).Result as OkObjectResult)?.Value;

        Assert.NotNull(result);
        Assert.IsType<RegistrationResponseModel>(result);
        Assert.False((result as RegistrationResponseModel)?.Success);
    }
    [Fact]
    public async Task UpdateUserProfile_ReturnsNoContentResult()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());
        _accountServiceMock.Setup(a => a.UpdateUserProfileAsync(It.IsAny<string>(), It.IsAny<UserProfileUpdateModel>()))
            .Callback(() => { });

        var result = await _controller.UpdateUserProfile("newName", new UserProfileUpdateModel());

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task UpdateUserProfile_ReturnsNoContentResult_WhenCorrectlyUpdatesUsername()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel() { UserName = "oldName" });
        _userServiceMock.Setup(u => u.UpdateUserNameAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Callback(() => { });
        _accountServiceMock.Setup(a => a.UpdateUserProfileAsync(It.IsAny<string>(), It.IsAny<UserProfileUpdateModel>()))
            .Callback(() => { });

        var result = await _controller.UpdateUserProfile("oldName", new UserProfileUpdateModel() { UserName = "newName" });

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task UpdateUserProfile_ReturnsNoContentResult_WhenCorrectlyUpdatesUserRoles()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());
        _userServiceMock.Setup(a => a.UpdateUserRolesAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
            .Callback(() => { });
        _accountServiceMock.Setup(a => a.UpdateUserProfileAsync(It.IsAny<string>(), It.IsAny<UserProfileUpdateModel>()))
            .Callback(() => { });
        var model = new UserProfileUpdateModel() { Roles = new List<string>() { "firstRole", "secondRole" } };

        var result = await _controller.UpdateUserProfile("oldName", model);

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task UpdateUserProfile_ReturnsBadRequestObjectResult_IfModelWasInvalid()
    {
        var controller = new UsersController(_userServiceMock.Object, _accountServiceMock.Object,
            ControllersHelper.GetValidationService(false));

        var result = await controller.UpdateUserProfile("testUser", new UserProfileUpdateModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task UpdateUserProfile_ReturnsBadRequestObjectResult_IfTheUserProfileWasNotUpdated()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());
        _accountServiceMock.Setup(a => a.UpdateUserProfileAsync(It.IsAny<string>(), It.IsAny<UserProfileUpdateModel>()))
            .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.UpdateUserProfile("newName", new UserProfileUpdateModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task UpdateUserProfile_ReturnsBadRequestResult_IfUserDoesNotExist()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync((UserProfileModel?)null);

        var result = await _controller.UpdateUserProfile("newName", new UserProfileUpdateModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task DeleteUser_ReturnsNoContentResult()
    {
        _userServiceMock.Setup(u => u.DeleteUserAsync(It.IsAny<string>()))
            .Callback(() => { });

        var result = await _controller.DeleteUser("1");

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task ChangeUserPassword_ReturnsNoContentResult()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());
        _userServiceMock.Setup(a => a.ChangeUserPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Callback(() => { });

        var result = await _controller.ChangeUserPassword("1", new SetPasswordModel());

        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task ChangeUserPassword_ReturnsBadRequestObjectResult_IfModelWasInvalid()
    {
        var controller = new UsersController(_userServiceMock.Object, _accountServiceMock.Object,
            ControllersHelper.GetValidationService(false));

        var result = await controller.ChangeUserPassword("testUser", new SetPasswordModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task ChangeUserPassword_ReturnsBadRequestObjectResult_IfUserDoesNotExist()
    {
        _userServiceMock.Setup(a => a.ChangeUserPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.ChangeUserPassword("1", new SetPasswordModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task ChangeUserPassword_ReturnsBadRequestObjectResult_IfThePasswordHasNotBeenChanged()
    {
        _userServiceMock.Setup(u => u.GetUserByNameOrIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserProfileModel());
        _userServiceMock.Setup(a => a.ChangeUserPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("TestException"));

        var result = await _controller.ChangeUserPassword("1", new SetPasswordModel());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetAllRoles_ReturnsOkObjectResult()
    {
        _userServiceMock.Setup(u => u.GetAllRoles()).Returns(new List<string>());

        var result = (_controller.GetAllRoles()).Result;

        Assert.IsType<OkObjectResult>(result);
    }
}
