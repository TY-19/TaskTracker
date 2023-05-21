using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;

namespace TaskTracker.WebAPI.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly IValidationService _validationService;
    public UsersController(IUserService userService,
        IAccountService accountService,
        IValidationService validationService)
    {
        _userService = userService;
        _accountService = accountService;
        _validationService = validationService;
    }

    [HttpGet]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserProfileModel>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [Route("{userNameOrId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileModel>> GetUserProfile(string userNameOrId)
    {
        var user = await _userService.GetUserByNameOrIdAsync(userNameOrId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RegistrationResponseModel>> CreateUserProfile(
        RegistrationRequestModel model)
    {
        var validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return new RegistrationResponseModel()
            {
                Success = false,
                Message = $"Validation errors:{Environment.NewLine}{validationResult}"
            };

        return Ok(await _accountService.RegistrationAsync(model));
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE}")]
    [Route("{userNameOrId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUserProfile(string userNameOrId,
        UserProfileUpdateModel updatedUser)
    {
        var validationResult = _validationService.Validate(updatedUser);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        var userName = (await _userService.GetUserByNameOrIdAsync(userNameOrId))?.UserName;
        if (userName == null || updatedUser == null)
            return BadRequest("User cannot be updated");

        if (!await _accountService.UpdateUserProfileAsync(userName, updatedUser))
            return BadRequest("User was not updated");

        if (updatedUser.UserName != null && updatedUser.UserName != userName)
            await _userService.UpdateUserNameAsync(userName, updatedUser.UserName);

        return NoContent();
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE}")]
    [Route("{userNameOrId}/changepassword")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeUserPassword(string userNameOrId, SetPasswordModel model)
    {
        var validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        try
        {
            await _userService.ChangeUserPasswordAsync(userNameOrId, model.NewPassword);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE}")]
    [Route("{userNameOrId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser(string userNameOrId)
    {
        await _userService.DeleteUserAsync(userNameOrId);
        return NoContent();
    }
}
