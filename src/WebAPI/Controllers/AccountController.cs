using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;

namespace TaskTracker.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [Route("login")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequestModel loginRequest)
    {
        LoginResponseModel loginResult = await _accountService.LoginAsync(loginRequest);
        return Ok(loginResult);
    }

    [Route("registration")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RegistrationResponseModel>> Registration(RegistrationRequestModel registrationRequest)
    {
        RegistrationResponseModel registrationResponse =
            await _accountService.RegistrationAsync(registrationRequest);
        return Ok(registrationResponse);
    }

    [Authorize]
    [Route("profile")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileModel>> ViewProfile()
    {
        var userName = User?.Identity?.Name;
        if (userName == null)
            return NotFound();

        var userProfile = await _accountService.GetUserProfile(userName);
        if (userProfile == null)
            return NotFound();

        return Ok(userProfile);
    }

    [Authorize]
    [Route("profile")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(UserProfileUpdateModel updatedUser)
    {
        var userName = User?.Identity?.Name;
        if (userName == null)
            return NotFound();

        if (await _accountService.UpdateUserProfile(userName, updatedUser))
        {
            return NoContent();
        }
        else
        {
            return BadRequest();
        }
    }

    [Authorize]
    [Route("profile/changepassword")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        var userName = User?.Identity?.Name;
        if (userName == null)
            return NotFound();

        if (await _accountService.ChangePasswordAsync(userName, model))
        {
            return NoContent();
        }
        else
        {
            return BadRequest();
        }
    }
}
