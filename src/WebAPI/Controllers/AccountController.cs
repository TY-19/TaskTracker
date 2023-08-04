using FluentValidation.Results;
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
    private readonly IValidationService _validationService;
    public AccountController(IAccountService accountService,
        IValidationService validationService)
    {
        _accountService = accountService;
        _validationService = validationService;
    }

    [Route("login")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel loginRequest)
    {
        ValidationResult validationResult = _validationService.Validate(loginRequest);
        if (!validationResult.IsValid)
            return Ok(GetResponseToInvalidLoginRequest(validationResult));

        return Ok(await _accountService.LoginAsync(loginRequest));
    }
    private LoginResponseModel GetResponseToInvalidLoginRequest(ValidationResult validationResult)
    {
        return new LoginResponseModel()
            {
                Success = false,
                Message = $"Validation errors:{Environment.NewLine}{validationResult}"
            };
    }

    [Route("registration")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<RegistrationResponseModel>> Registration(
        RegistrationRequestModel registrationRequest)
    {
        ValidationResult validationResult = _validationService.Validate(registrationRequest);
        if (!validationResult.IsValid)
            return Ok(GetResponseToInvalidRegistrationRequest(validationResult));

        return Ok(await _accountService.RegistrationAsync(registrationRequest));
    }
    private RegistrationResponseModel GetResponseToInvalidRegistrationRequest(ValidationResult validationResult)
    {
        return new RegistrationResponseModel()
            {
                Success = false,
                Message = $"Validation errors:{Environment.NewLine}{validationResult}"
            };
    }

    [Authorize]
    [Route("profile")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileModel>> ViewProfile()
    {
        string? userName = User?.Identity?.Name;
        if (userName == null)
            return NotFound();

        UserProfileModel? userProfile = await _accountService.GetUserProfileAsync(userName);
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
        ValidationResult validationResult = _validationService.Validate(updatedUser);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        string? userName = User?.Identity?.Name;
        if (userName == null)
            return NotFound();

        try
        {
            await _accountService.UpdateUserProfileAsync(userName, updatedUser);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
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
        ValidationResult validationResult = _validationService.Validate(model);
        if (!validationResult.IsValid)
            return BadRequest($"Validation errors:{Environment.NewLine}{validationResult}");

        string? userName = User?.Identity?.Name;
        if (userName == null)
            return NotFound();

        try
        {
            await _accountService.ChangePasswordAsync(userName, model);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return NoContent();
    }
}
