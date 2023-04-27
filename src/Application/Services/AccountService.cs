using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Models;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtHandlerService _jwtHandlerService;
    private readonly IMapper _mapper;
    private readonly ITrackerDbContext _context;
    private readonly IValidator<RegistrationRequestModel> _validator;
    public AccountService(UserManager<User> userManager,
        JwtHandlerService jwtHandlerService,
        IMapper mapper,
        ITrackerDbContext context,
        IValidator<RegistrationRequestModel> validator)
    {
        _userManager = userManager;
        _jwtHandlerService = jwtHandlerService;
        _mapper = mapper;
        _context = context;
        _validator = validator;
    }

    public async Task<LoginResponseModel> LoginAsync(LoginRequestModel loginRequest)
    {
        var user = await FindUserAsync(loginRequest.NameOrEmail);

        if (user is null || !await CheckPasswordAsync(user, loginRequest.Password))
            return GetLoginFailedResult();

        var token = await _jwtHandlerService.GetTokenAsync(user);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return GetLoginSucceedResult(jwt);
    }

    public async Task<RegistrationResponseModel> RegistrationAsync(
        RegistrationRequestModel registrationRequest)
    {
        var validationResult = _validator.Validate(registrationRequest);
        if (!validationResult.IsValid)
            return GetRegistrationResult(false, validationResult.ToString());

        if (await _userManager.FindByNameAsync(registrationRequest.UserName) != null)
            return GetRegistrationResult(false, "User with this name already exists");

        var user = new User()
        {
            UserName = registrationRequest.UserName,
            Email = registrationRequest.Email,
            Employee = new Employee()
        };
        try
        {
            await _userManager.CreateAsync(user, registrationRequest.Password);
            await _userManager.AddToRoleAsync(user, DefaultRolesNames.DEFAULT_EMPLOYEE_ROLE);
        }
        catch
        {
            return GetRegistrationResult(false, "Name or password is not valid");
        }
        return GetRegistrationResult(true, "Registration was successfull");
    }

    private static RegistrationResponseModel GetRegistrationResult(bool success, string message)
    {
        return new RegistrationResponseModel() 
        { 
            Success = success, 
            Message = message 
        };
    }

    public async Task<UserProfileModel?> GetUserProfileAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);

        return user == null 
            ? null 
            : _mapper.Map<UserProfileModel>(user);
    }

    public async Task<bool> UpdateUserProfileAsync(string userName,
        UserProfileUpdateModel updatedUser)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
            return false;

        await UpdateUserInfo(user, updatedUser);
        await _userManager.UpdateAsync(user);

        return true;
    }

    private async Task UpdateUserInfo(User user, UserProfileUpdateModel updatedUser)
    {
        if (updatedUser?.Email != null)
        {
            user.Email = updatedUser.Email;
        }
        if (user.Employee == null)
        {
            var employee = new Employee();
            await _context.Employees.AddAsync(employee);
            user.Employee = employee;
        }
        if (updatedUser?.FirstName != null)
        {
            user.Employee.FirstName = updatedUser.FirstName;
        }
        if (updatedUser?.LastName != null)
        {
            user.Employee.LastName = updatedUser.LastName;
        }
    }

    public async Task<bool> ChangePasswordAsync(string userName,
        ChangePasswordModel model)
    {
        User user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return false;
        }
        try
        {
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            return result.Succeeded;
        }
        catch
        {
            return false;
        }
    }

    private async Task<User?> FindUserAsync(string nameOrEmail)
    {
        User user;
        if (nameOrEmail.Contains('@'))
        {
            user = await _userManager.FindByEmailAsync(nameOrEmail);
        }
        else
        {
            user = await _userManager.FindByNameAsync(nameOrEmail);
        }
        return user;
    }

    private async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    private static LoginResponseModel GetLoginFailedResult()
    {
        return new LoginResponseModel
        {
            Success = false,
            Message = "Invalid Name/Email or Password",
            Token = null
        };
    }

    private static LoginResponseModel GetLoginSucceedResult(string token)
    {
        return new LoginResponseModel
        {
            Success = true,
            Message = "Login successful",
            Token = token
        };
    }
}
