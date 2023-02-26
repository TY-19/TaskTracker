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
    public UsersController(IUserService userService,
        IAccountService accountService)
    {
        _userService = userService;
        _accountService = accountService;
    }

    [HttpGet]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<IEnumerable<UserProfileModel>> GetAllUsers()
    {
        var users = _userService.GetAllUsers();
        return Ok(users);
    }

    [Route("{userNameOrId}")]
    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE},{DefaultRolesNames.DEFAULT_MANAGER_ROLE}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserProfile(string userNameOrId)
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
        var result = await _accountService.RegistrationAsync(model);
        return Ok(result);
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
        var userName = _userService.GetUserByNameOrIdAsync(userNameOrId).Result?.UserName;
        if (userName == null || updatedUser == null)
            return BadRequest();

        await _accountService.UpdateUserProfile(userName, updatedUser);

        if (updatedUser.UserName != null && updatedUser.UserName != userName)
            await _userService.UpdateUserName(userName, updatedUser.UserName);

        return NoContent();
    }

    [Authorize(Roles = $"{DefaultRolesNames.DEFAULT_ADMIN_ROLE}")]
    [Route("{userNameOrId}/changepassword")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeUserPassword(string userNameOrId, SetPasswordModel model)
    {
        var user = await _userService.GetUserByNameOrIdAsync(userNameOrId);
        if (user == null)
            return BadRequest();
        
        await _userService.ChangeUserPassword(userNameOrId, model.NewPassword);

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
        await _userService.DeleteUser(userNameOrId);
        return NoContent();
    }
}
