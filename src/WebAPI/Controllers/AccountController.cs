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
    public async Task<IActionResult> Login(LoginRequestModel loginRequest)
    {
        LoginResponseModel loginResult = await _accountService.LoginAsync(loginRequest);
        return Ok(loginResult);
    }

    [Route("registration")]
    [HttpPost]
    public async Task<IActionResult> Registration(RegistrationRequestModel registrationRequest)
    {
        RegistrationResponseModel registrationResponse =
            await _accountService.RegistrationAsync(registrationRequest);
        return Ok(registrationResponse);
    }

    [Authorize]
    [Route("profile")]
    [HttpGet]
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
}
